using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : MyMapsterBaseController<StudentsController, SkoleContext>
    {
        public StudentsController(SkoleContext db, ILogger<StudentsController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
            => await ProjectListAsync<Student, StudentDto>(_db.Studerende);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> Get(int id)
            => await GetByIdAsync<Student, StudentDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> Search(string navn)
            => await GetFilteredAsync<Student, StudentDto>(s => s.Navn.Contains(navn));

        // 4. Opret (Create)
        [HttpPost("CreateStudent")]
        public async Task<ActionResult<StudentDto>> CreateStudent(StudentCreateDto dto)
            => await CreateAsync<Student, StudentCreateDto, StudentDto>(dto, nameof(Get));

        [HttpPost("CreateStudentWithMultipleRelations")]
        public async Task<ActionResult<StudentDto>> CreateStudentWithMultipleRelations(StudentCreateDto dto)
        {
            return await CreateWithMultipleRelationsAsync<Student, StudentCreateDto, StudentDto>(
            dto,
            nameof(Get),
            // Relation 1: Fag
            async (student) =>
            {
                var collection = _db.Entry(student).Collection(s => s.Fag).CurrentValue as ICollection<Fag> ?? new List<Fag>();
                await SynchronizeManyToManyAsync(collection, dto.FagIds, f => f.FagId);
            }//,
            // Hvis der var en mange-til-mange releation mellem Students og Teachers
            // Relation 2: Teacher
            //async (student) => {
            //    var collection = _db.Entry(student).Collection(s => s.Teacher).CurrentValue ?? new List<Teacher>();
            //    await SynchronizeManyToManyAsync(collection, dto.TeacherIds, t => t.TeacherId);
            //}
            );
        }


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateWithRelations/{id}")]
        public async Task<IActionResult> UpdateWithRelations(int id, StudentUpdateDto dto)
        {
            // Vi bruger den avancerede base-metode til at synkronisere 'Fag'
            // s.Fag er navnet på listen i din Student-entitet
            // dto.FagIds er listen med ID'er fra din DTO
            // f.FagId er navnet på nøglen i Fag-entiteten
            return await UpdateWithRelationsAsync<Student, StudentUpdateDto, Fag, int>(
                id,
                dto,
                s => s.Fag,
                dto.FagIds,
                f => f.FagId);
        }

        [HttpPut("UpdateWithMultipleRelations/{id}")]
        //public async Task<IActionResult> UpdateWithMultipleRelations(int id, StudentUpdateDto dto)
        //{
        //    return await UpdateWithMultipleRelationsAsync<Student, StudentUpdateDto>(
        //        id,
        //        dto,
        //        // Opgave 1: Synkronisér Fag
        //        async (student) =>
        //        {
        //            var collection = _db.Entry(student).Collection(s => s.Fag).CurrentValue as ICollection<Fag>;
        //            if (collection == null)
        //            {
        //                await _db.Entry(student).Collection(s => s.Fag).LoadAsync();
        //                collection = student.Fag;
        //            }
        //            await SynchronizeManyToManyAsync(collection, dto.FagIds, f => f.FagId);
        //        }
        //        //// Opgave 2: Synkronisér Lærere
        //        //async (student) => {
        //        //    var collection = _db.Entry(student).Collection(s => s.Laerere).CurrentValue;
        //        //    if (collection == null)
        //        //    {
        //        //        await _db.Entry(student).Collection(s => s.Laerere).LoadAsync();
        //        //        collection = student.Laerere;
        //        //    }
        //        //    await SynchronizeManyToManyAsync(collection, dto.LaererIds, l => l.Id);
        //        //}
        //    );
        //}

        [HttpPut("{id}")]
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
                //// Relation 2: Lærere (hvis relevant)
                //async (student) => {
                //    var collectionEntry = _db.Entry(student).Collection(s => s.Laerere);
                //    if (!collectionEntry.IsLoaded) await collectionEntry.LoadAsync();

                //    var collection = collectionEntry.CurrentValue as ICollection<Laerer>;
                //    if (collection != null)
                //    {
                //        await SynchronizeManyToManyAsync(collection, dto.LaererIds, l => l.Id);
                //    }
                //}
            );
        }



        // 6. Slet (Delete)
        [HttpDelete("DeleteSimpleObject/{id}")]
        public async Task<IActionResult> DeleteSimpleObject(int id)
            => await DeleteAsync<Student>(id);

        [HttpDelete("DeleteObjectWithMultipleRelations/{id}")]
        public async Task<IActionResult> DeleteObjectWithMultipleRelations(int id)
        {
            // Vi beder basen om at rydde op i Fag og Laerere samlingerne før sletning
            return await DeleteWithMultipleRelationsAsync<Student>(id,
                s => s.Fag//,
                //s => s.Laerere
            );
        }
    }
}
