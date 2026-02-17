using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : MyMapsterBaseController<TeachersController, SkoleContext>
    {
        public TeachersController(SkoleContext db, ILogger<TeachersController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll()
            => await ProjectListAsync<Teacher, TeacherDto>(_db.Teachers);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> Get(int id)
            => await GetByIdAsync<Teacher, TeacherDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> Search(string navn)
            => await GetFilteredAsync<Teacher, TeacherDto>(t => t.Afdelinger.Any(f => f.AfdelingNavn.Contains(navn)));

        // 4. Opret (Create)
        [HttpPost("CreateTeacherWithMultipleRelations")]
        public async Task<ActionResult<TeacherDto>> CreateTeacherWithMultipleRelations(TeacherCreateDto dto)
        {
            return await CreateWithMultipleRelationsAsync<Teacher, TeacherCreateDto, TeacherDto>(
                dto,
                nameof(Get),
                // Relation 1: Fag
                async (teacher) =>
                {
                    var collection = _db.Entry(teacher).Collection(t => t.Fag).CurrentValue as ICollection<Fag> ?? new List<Fag>();
                    await SynchronizeManyToManyAsync(collection, dto.FagIds, f => f.FagId);
                },
                async (teacher) =>
                {
                    var collection = _db.Entry(teacher).Collection(t => t.Afdelinger).CurrentValue as ICollection<Afdeling> ?? new List<Afdeling>();
                    await SynchronizeManyToManyAsync(collection, dto.AfdelingsIds, a => a.AfdelingId);
                }
            );
        }


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateWithMultipleRelations/{id}")]
        public async Task<IActionResult> UpdateWithMultipleRelations(int id, TeacherUpdateDto dto)
        {
            return await UpdateWithMultipleRelationsAsync<Teacher, TeacherUpdateDto>(
                id,
                dto,
                // Relation 1: Fag
                async (teacher) => {
                    // 1. Hent collection entry fra EF
                    var collectionEntry = _db.Entry(teacher).Collection(t => t.Fag);

                    // 2. VIGTIGT: Sørg for at data er indlæst fra DB. 
                    // Hvis listen er tom i hukommelsen, vil Synchronize tro at alt er nyt og lave dubletter.
                    if (!collectionEntry.IsLoaded)
                    {
                        await collectionEntry.LoadAsync();
                    }

                    // 3. Cast til ICollection<Fag> for at løse CS0411 (Type Inference)
                    var collection = collectionEntry.CurrentValue as ICollection<Fag>;

                    if (collection != null)
                    {
                        // 4. Nu kan vi sikkert synkronisere uden PK-fejl
                        await SynchronizeManyToManyAsync(collection, dto.FagIds, f => f.FagId);
                    }
                },
                // Relation 2: Afdelinger 
                async (teacher) =>
                {
                    var collectionEntry = _db.Entry(teacher).Collection(t => t.Afdelinger);
                    if (!collectionEntry.IsLoaded) await collectionEntry.LoadAsync();

                    var collection = collectionEntry.CurrentValue as ICollection<Afdeling>;
                    if (collection != null)
                    {
                        await SynchronizeManyToManyAsync(collection, dto.AfdelingsIds, a => a.AfdelingId);
                    }
                }
            );
        }

        // 6. Slet (Delete)
        [HttpDelete("DeleteObjectWithMultipleRelations/{id}")]
        public async Task<IActionResult> DeleteObjectWithMultipleRelations(int id)
        {
            // Vi beder basen om at rydde op i Fag og Laerere samlingerne før sletning
            return await DeleteWithMultipleRelationsAsync<Teacher>(id,
                t => t.Fag,
                a => a.Afdelinger
            );
        }
    }
}
