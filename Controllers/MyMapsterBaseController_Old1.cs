using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        protected async Task<ActionResult<IEnumerable<TDto>>> ProjectListAsync<TEntity, TDto>(
            IQueryable<TEntity> query) where TEntity : class
        {
            // ProjectToType sørger for at SQL'en kun henter de nødvendige felter (fx Navn, HoldNavn)
            return Ok(await query.ProjectToType<TDto>().ToListAsync());
        }

        protected async Task<ActionResult<TDto>> ProjectSingleAsync<TEntity, TDto>(
            IQueryable<TEntity> query) where TEntity : class
        {
            var item = await query.ProjectToType<TDto>().FirstOrDefaultAsync();
            return item == null ? NotFound() : Ok(item);
        }

        /// <summary>
        /// Henter en liste baseret på et filter (f.eks. "ForeignKey ID")
        /// </summary>
        protected async Task<ActionResult<IEnumerable<TDto>>> GetFilteredAsync<TEntity, TDto>(
            Expression<Func<TEntity, bool>> predicate,
            bool useTracking = false) where TEntity : class
        {
            IQueryable<TEntity> query = _db.Set<TEntity>().Where(predicate);

            if (!useTracking) query = query.AsNoTracking();

            var list = await query.ProjectToType<TDto>().ToListAsync();
            return Ok(list);
        }

        // --- DB HANDLINGER (POST, PUT, DELETE) ---

        protected async Task<ActionResult<TDto>> CreateAsync<TEntity, TCreateDto, TDto>(TCreateDto dto, string actionName)
            where TEntity : class
        {
            var entity = dto.Adapt<TEntity>(); // Mapster instantiere korrekt type baseret på TEntity
            _db.Set<TEntity>().Add(entity);
            await _db.SaveChangesAsync();

            var resultDto = entity.Adapt<TDto>();
            return CreatedAtAction(actionName, new { id = (entity as dynamic).Id }, resultDto);
        }

        //protected async Task<IActionResult> UpdateAsync<TEntity, TUpdateDto>(int id, TUpdateDto dto)
        //    where TEntity : class
        //{
        //    var entity = await _db.Set<TEntity>().FindAsync(id);
        //    if (entity == null) return NotFound();

        //    // Mapster opdaterer eksisterende entity. 
        //    // Ved N:M relationer skal man være varsom her, hvis DTO indeholder hele lister.
        //    dto.Adapt(entity);

        //    return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
        //}

        protected async Task<IActionResult> UpdateWithRelationsAsync<TEntity, TUpdateDto, TRelatedEntity, TKey>(
            int id,
            TUpdateDto dto,
            Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> collectionSelector,
            List<TKey> newIds,
            Func<TRelatedEntity, TKey> keySelector)
            where TEntity : class
            where TRelatedEntity : class
            where TKey : notnull
        {
            // Find PK dynamisk (f.eks. StudentId)
            var pkName = _db.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.First().Name ?? "Id";

            // Hent entiteten og inkluder den ønskede samling (f.eks. Fag)
            var entity = await _db.Set<TEntity>()
                .Include(collectionSelector)
                .FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);

            if (entity == null) return NotFound();

            // Map simple felter (undtagen navigation properties som Mapster er sat til at ignorere)
            dto.Adapt(entity);

            // Hent selve samlingen (f.eks. student.Fag)
            var currentCollection = collectionSelector.Compile()(entity) as ICollection<TRelatedEntity>;

            if (currentCollection != null)
            {
                await SynchronizeManyToManyAsync(currentCollection, newIds, keySelector);
            }

            return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
        }

        // --- SLETNING (DELETE) ---

        /// <summary>
        /// Sletter en entitet baseret på dens Primary Key (f.eks. StudentId)
        /// </summary>
        protected async Task<IActionResult> DeleteAsync<TEntity>(int id) where TEntity : class
        {
            // Find PK navnet via EF metadata (f.eks. "StudentId")
            var pkName = _db.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault() ?? "Id";

            // Find entiteten ved at bruge EF.Property til at matche dynamisk navngivet nøgle
            var entity = await _db.Set<TEntity>()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);

            if (entity == null) return NotFound();

            return await ExecuteDbActionAsync(async () =>
            {
                _db.Set<TEntity>().Remove(entity);
                await _db.SaveChangesAsync();
            });
        }

        protected async Task<IActionResult> ExecuteDbActionAsync(Func<Task> action)
        {
            try
            {
                await action();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Databasefejl");
                return BadRequest("Fejl ved lagring af data. Tjek relationer.");
            }
        }

        //protected async Task SynchronizeManyToManyAsync<TRelatedEntity>(
        //    ICollection<TRelatedEntity> currentItems,
        //    List<int> newIds)
        //    where TRelatedEntity : class
        //{
        //    // 1. Find Primary Key navnet (typisk "Id") via EF Metadata eller antag "Id"
        //    // For enkelthedens skyld antager vi her en ejendom ved navn 'Id'
        //    var idProperty = typeof(TRelatedEntity).GetProperty("Id");
        //    if (idProperty == null) throw new InvalidOperationException("Entiteten skal have en 'Id' property.");

        //    // Helper til at få ID fra en entitet
        //    int GetId(TRelatedEntity item) => (int)idProperty.GetValue(item)!;

        //    // 2. Fjern de emner, der ikke længere er i de nye ID'er
        //    var itemsToRemove = currentItems.Where(item => !newIds.Contains(GetId(item))).ToList();
        //    foreach (var item in itemsToRemove)
        //    {
        //        currentItems.Remove(item);
        //    }

        //    // 3. Find de ID'er, der skal tilføjes
        //    var existingIds = currentItems.Select(GetId).ToList();
        //    var idsToAdd = newIds.Except(existingIds).ToList();

        //    if (idsToAdd.Any())
        //    {
        //        // Hent de nye entiteter fra databasen
        //        var newEntities = await _db.Set<TRelatedEntity>()
        //            .Where(e => idsToAdd.Contains(EF.Property<int>(e, "Id")))
        //            .ToListAsync();

        //        foreach (var entity in newEntities)
        //        {
        //            currentItems.Add(entity);
        //        }
        //    }
        //}

        protected async Task SynchronizeManyToManyAsync<TRelatedEntity, TKey>(
        ICollection<TRelatedEntity> currentItems,
        List<TKey> newIds,
        Func<TRelatedEntity, TKey> keySelector)
        where TRelatedEntity : class
        where TKey : notnull
        {
            // 1. Fjern de emner, der ikke længere er i listen af nye ID'er
            var itemsToRemove = currentItems
                .Where(item => !newIds.Contains(keySelector(item)))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                currentItems.Remove(item);
            }

            // 2. Find de ID'er, der skal tilføjes (dem der er i newIds, men ikke i currentItems)
            var existingIds = currentItems.Select(keySelector).ToList();
            var idsToAdd = newIds.Except(existingIds).ToList();

            if (idsToAdd.Any())
            {
                // Vi bruger EF Core's metadata til at finde navnet på PK-feltet dynamisk 
                // så vi kan lave en effektiv database-forespørgsel
                var entityType = _db.Model.FindEntityType(typeof(TRelatedEntity));
                var primaryKeyName = entityType?.FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault()
                                     ?? "Id"; // Fallback hvis alt fejler

                var newEntities = await _db.Set<TRelatedEntity>()
                    .Where(e => idsToAdd.Contains(EF.Property<TKey>(e, primaryKeyName)))
                    .ToListAsync();

                foreach (var entity in newEntities)
                {
                    currentItems.Add(entity);
                }
            }
        }

        protected async Task<ActionResult<TDto>> GetByIdAsync<TEntity, TDto>(int id) where TEntity : class
        {
            // Den skudsikre logik: Find PK navnet automatisk (fx PersonId eller StudentId)
            var pkName = _db.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault() ?? "Id";

            // Søg direkte i databasen ved hjælp af den fundne nøgle
            return await ProjectSingleAsync<TEntity, TDto>(
                _db.Set<TEntity>().Where(e => EF.Property<int>(e, pkName) == id)
            );
        }

        // Hjælpefunktion til at finde Primary Key navnet (fx "PersonId") via EF Metadata
        private string GetPrimaryKeyName<TEntity>()
        {
            return _db.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?.Properties.Select(p => p.Name).FirstOrDefault() ?? "Id";
        }
    }
}
