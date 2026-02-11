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
    public class StudentController : MyMapsterBaseController<StudentController, SkoleContext>
    {
        public StudentController(SkoleContext db, ILogger<StudentController> logger)
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
        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create(StudentCreateDto dto)
            => await CreateAsync<Student, StudentCreateDto, StudentDto>(dto, nameof(Get));

        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StudentUpdateDto dto)
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

        // 6. Slet (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await DeleteAsync<Student>(id);
    }
}
