using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;
using System.Linq.Expressions;
using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class MyMapsterBaseController<TController, TContext> : ControllerBase
        where TContext : DbContext
    {
        protected readonly TContext _db;
        protected readonly ILogger<TController> _logger;

        protected MyMapsterBaseController(TContext db, ILogger<TController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // --- LÆSNING (GET) ---

        protected async Task<ActionResult<TDto>> GetByIdAsync<TEntity, TDto>(int id) where TEntity : class
        {
            var pkName = GetPrimaryKeyName<TEntity>();
            // Her bruger vi dynamisk PK navn fra EF Metadata - virker både med "Id" og "FagId"
            var query = _db.Set<TEntity>().Where(e => EF.Property<int>(e, pkName) == id);
            return await ProjectSingleAsync<TEntity, TDto>(query);
        }

        protected async Task<ActionResult<IEnumerable<TDto>>> ProjectListAsync<TEntity, TDto>(
            IQueryable<TEntity> query) where TEntity : class
            => Ok(await query.AsNoTracking().ProjectToType<TDto>().ToListAsync());

        protected async Task<ActionResult<TDto>> ProjectSingleAsync<TEntity, TDto>(
            IQueryable<TEntity> query) where TEntity : class
        {
            var item = await query.AsNoTracking().ProjectToType<TDto>().FirstOrDefaultAsync();
            return item == null ? NotFound() : Ok(item);
        }

        protected async Task<ActionResult<IEnumerable<TDto>>> GetFilteredAsync<TEntity, TDto>(
            Expression<Func<TEntity, bool>> predicate) where TEntity : class
            => Ok(await _db.Set<TEntity>().AsNoTracking().Where(predicate).ProjectToType<TDto>().ToListAsync());

        // --- DB HANDLINGER (POST, PUT, DELETE) ---

        // POST

        protected async Task<ActionResult<TDto>> CreateAsync<TEntity, TCreateDto, TDto>(TCreateDto dto, string actionName)
            where TEntity : class
        {
            var entity = dto.Adapt<TEntity>();
            _db.Set<TEntity>().Add(entity);
            await _db.SaveChangesAsync();

            var pkName = GetPrimaryKeyName<TEntity>();
            var idValue = _db.Entry(entity).Property(pkName).CurrentValue;

            return CreatedAtAction(actionName, new { id = idValue }, entity.Adapt<TDto>());
        }

        protected async Task<ActionResult<TDto>> CreateWithMultipleRelationsAsync<TEntity, TCreateDto, TDto>(
            TCreateDto dto,
            string actionName,
            params Func<TEntity, Task>[] relationSyncs) // Vi sender en liste af opgaver med
            where TEntity : class
        {
            // Start Transaction
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = dto.Adapt<TEntity>();
                _db.Set<TEntity>().Add(entity);
                await _db.SaveChangesAsync(); // Gem for at få ID

                // Kør alle relation-synkroniseringer (fx både Fag og Lærere)
                foreach (var sync in relationSyncs)
                {
                    await sync(entity);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync(); // Godkend alt

                var pkName = GetPrimaryKeyName<TEntity>();
                var idValue = _db.Entry(entity).Property(pkName).CurrentValue;
                return CreatedAtAction(actionName, new { id = idValue }, entity.Adapt<TDto>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Fortryd alt hvis noget fejler
                _logger.LogError(ex, "Fejl under Create med relationer");
                return BadRequest("Kunne ikke oprette objektet.");
            }
        }

        // PUT

        // Opdateret UpdateAsync med IHasIdField interface check
        protected async Task<IActionResult> UpdateAsync<TEntity, TUpdateDto>(int id, TUpdateDto dto)
            where TEntity : class
            where TUpdateDto : IHasIdField
        {
            // Sikkerhed: Tjek om ID i URL matcher ID i DTO (selvom DTO feltet hedder .Id)
            if (id != dto.Id) return BadRequest("ID mismatch mellem URL og krop.");

            var pkName = GetPrimaryKeyName<TEntity>();
            var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);

            if (entity == null) return NotFound();

            dto.Adapt(entity);
            return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
        }

        // Opdateret UpdateWithRelationsAsync med IHasIdField interface check
        protected async Task<IActionResult> UpdateWithRelationsAsync<TEntity, TUpdateDto, TRelatedEntity, TKey>(
            int id,
            TUpdateDto dto,
            Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> collectionSelector,
            List<TKey> newIds,
            Func<TRelatedEntity, TKey> keySelector)
            where TEntity : class 
            where TRelatedEntity : class 
            where TKey : notnull
            where TUpdateDto : IHasIdField
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");

            using var transaction = await _db.Database.BeginTransactionAsync();

            var pkName = GetPrimaryKeyName<TEntity>();
            var entity = await _db.Set<TEntity>()
                .Include(collectionSelector)
                .FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);

            if (entity == null) return NotFound();

            dto.Adapt(entity);

            var currentCollection = collectionSelector.Compile()(entity) as ICollection<TRelatedEntity>;
            if (currentCollection != null)
            {
                await SynchronizeManyToManyAsync(currentCollection, newIds, keySelector);
            }

            //return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }

        protected async Task<IActionResult> UpdateWithMultipleRelationsAsync<TEntity, TUpdateDto>(
            int id,
            TUpdateDto dto,
            params Func<TEntity, Task>[] relationSyncs)
            where TEntity : class
            where TUpdateDto : IHasIdField
        {
            // Sikkerhedscheck: ID i URL skal matche DTO
            if (id != dto.Id) return BadRequest("ID mismatch.");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pkName = GetPrimaryKeyName<TEntity>(); // Finder automatisk "Id", "FagId" osv.

                // Vi henter entiteten. 
                // Note: Her behøver vi ikke .Include() på forhånd, 
                // da relationSyncs selv kan tilgå Collection Entries.
                var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);
                if (entity == null) return NotFound();

                // 1. Opdater simple felter (Navn, Alder, osv.) via Mapster
                dto.Adapt(entity);

                // 2. Kør alle mange-til-mange relation-synkroniseringer (Fag, Lærere, osv.)
                foreach (var sync in relationSyncs)
                {
                    await sync(entity);
                }

                // 3. Gem og bekræft
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Fejl under Update med multiple relationer");
                return BadRequest("Der opstod en fejl ved opdatering af relationerne.");
            }
        }

        // DELETE

        protected async Task<IActionResult> DeleteAsync<TEntity>(int id) where TEntity : class
        {
            var pkName = GetPrimaryKeyName<TEntity>();
            var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);
            if (entity == null) return NotFound();

            _db.Set<TEntity>().Remove(entity);
            return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
        }

        protected async Task<IActionResult> DeleteWithMultipleRelationsAsync<TEntity>(
            int id,
            params Expression<Func<TEntity, IEnumerable<object>>>[] collectionsToClear)
            where TEntity : class
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pkName = GetPrimaryKeyName<TEntity>();
                var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);

                if (entity == null) return NotFound();

                // 1. Indlæs og ryd alle specificerede mange-til-mange relationer
                foreach (var collectionSelector in collectionsToClear)
                {
                    var entry = _db.Entry(entity).Collection(collectionSelector);
                    await entry.LoadAsync(); // Hent relationerne fra DB

                    //if (entry.CurrentValue != null)
                    //{
                    //    entry.CurrentValue.Clear(); // Fjern alle links i bro-tabellen
                    //}
                    if (entry.CurrentValue is ICollection<object> collection)
                    {
                        collection.Clear(); // Fjern alle links i bro-tabellen
                    }
                }

                // 2. Slet selve objektet
                _db.Set<TEntity>().Remove(entity);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Fejl under sletning af objekt med relationer");
                return BadRequest("Kunne ikke slette objektet pga. afhængigheder.");
            }
        }

        // --- INTERNE HJÆLPERE ---

        private string GetPrimaryKeyName<TEntity>() =>
            _db.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault() ?? "Id";

        protected async Task SynchronizeManyToManyAsync<TRelatedEntity, TKey>(
            ICollection<TRelatedEntity> currentItems, List<TKey> newIds, Func<TRelatedEntity, TKey> keySelector)
            where TRelatedEntity : class where TKey : notnull
        {
            var itemsToRemove = currentItems.Where(item => !newIds.Contains(keySelector(item))).ToList();
            foreach (var item in itemsToRemove) currentItems.Remove(item);

            var existingIds = currentItems.Select(keySelector).ToList();
            var idsToAdd = newIds.Except(existingIds).ToList();

            if (idsToAdd.Any())
            {
                var relatedPkName = GetPrimaryKeyName<TRelatedEntity>();
                var newEntities = await _db.Set<TRelatedEntity>()
                    .Where(e => idsToAdd.Contains(EF.Property<TKey>(e, relatedPkName))).ToListAsync();
                foreach (var entity in newEntities) currentItems.Add(entity);
            }
        }

        protected async Task<IActionResult> ExecuteDbActionAsync(Func<Task> action)
        {
            try { await action(); return NoContent(); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved databaseoperation");
                return BadRequest("Der opstod en fejl i databasen.");
            }
        }
    }
}
