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

            return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
        }

        protected async Task<IActionResult> DeleteAsync<TEntity>(int id) where TEntity : class
        {
            var pkName = GetPrimaryKeyName<TEntity>();
            var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, pkName) == id);
            if (entity == null) return NotFound();

            _db.Set<TEntity>().Remove(entity);
            return await ExecuteDbActionAsync(async () => await _db.SaveChangesAsync());
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
