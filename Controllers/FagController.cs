using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FagController : MyMapsterBaseController<FagController, SkoleContext>
    {
        public FagController(SkoleContext db, ILogger<FagController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FagDto>>> GetAll()
            => await ProjectListAsync<Fag, FagDto>(_db.Fag);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<FagDto>> Get(int id)
            => await GetByIdAsync<Fag, FagDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FagDto>>> Search(string titel)
            => await GetFilteredAsync<Fag, FagDto>(f => f.FagTitel.Contains(titel));

        // 4. Opret (Create)
        [HttpPost("CreateFag")]
        public async Task<ActionResult<FagUpdateDto>> CreateFag(FagCreateDto dto)
            => await CreateAsync<Fag, FagCreateDto, FagUpdateDto>(dto, nameof(Get));


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateFag/{id}")]
        public async Task<IActionResult> UpdateFag(int id, FagUpdateDto dto)
        {
            // Vi bruger den avancerede base-metode til at synkronisere 'Fag'
            // s.Fag er navnet på listen i din Student-entitet
            // dto.FagIds er listen med ID'er fra din DTO
            // f.FagId er navnet på nøglen i Fag-entiteten
            return await UpdateAsync<Fag, FagUpdateDto>(
                id,
                dto);
        }


        // 6. Slet (Delete)
        [HttpDelete("DeleteSimpleObject/{id}")]
        public async Task<IActionResult> DeleteSimpleObject(int id)
            => await DeleteAsync<Fag>(id);
    }
}
