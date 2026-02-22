using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EUDStudentsController : MyMapsterBaseController<EUDStudentsController, SkoleContext>
    {
        public EUDStudentsController(SkoleContext db, ILogger<EUDStudentsController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EUDStudentDto>>> GetAll()
            => await ProjectListAsync<EUDStudent, EUDStudentDto>(_db.EUDStuderende);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<EUDStudentDto>> Get(int id)
            => await GetByIdAsync<EUDStudent, EUDStudentDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EUDStudentDto>>> Search(string navn)
            => await GetFilteredAsync<EUDStudent, EUDStudentDto>(s => s.Navn.Contains(navn));

        // 4. Opret (Create)
        [HttpPost("Create")]
        public async Task<ActionResult<EUDStudentDto>> Create(EUDStudentCreateDto dto)
            => await CreateAsync<EUDStudent, EUDStudentCreateDto, EUDStudentDto>(dto, nameof(Get));

        [HttpPost("CreateWithMultipleRelations")]
        public async Task<ActionResult<EUDStudentDto>> CreateWithMultipleRelations(EUDStudentCreateDto dto)
        {
            return await CreateWithMultipleRelationsAsync<EUDStudent, EUDStudentCreateDto, EUDStudentDto>(
            dto,
            nameof(Get),
            // Relation 1: Fag
            async (student) =>
            {
                var collection = _db.Entry(student).Collection(s => s.Fag).CurrentValue as ICollection<Fag> ?? new List<Fag>();
                await SynchronizeManyToManyAsync(collection, dto.FagIds, f => f.FagId);
            }
            );
        }


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateWithMultipleRelations/{id}")]
        public async Task<IActionResult> UpdateWithMultipleRelations(int id, StudentUpdateDto dto)
        {
            return await UpdateWithMultipleRelationsAsync<Student, StudentUpdateDto>(
                id,
                dto,
                // Relation 1: Fag
                async (student) => {
                    // 1. Hent collection entry fra EF
                    var collectionEntry = _db.Entry(student).Collection(s => s.Fag);

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
                }
            );
        }

        // 6. Slet (Delete)
        [HttpDelete("DeleteObjectWithMultipleRelations/{id}")]
        public async Task<IActionResult> DeleteObjectWithMultipleRelations(int id)
        {
            // Vi beder basen om at rydde op i Fag og Laerere samlingerne før sletning
            return await DeleteWithMultipleRelationsAsync<EUDStudent>(id,
                s => s.Fag
            );
        }
    }
}
