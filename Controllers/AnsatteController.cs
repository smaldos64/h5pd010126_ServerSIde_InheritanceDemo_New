using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnsatteController : MyMapsterBaseController<AnsatteController, SkoleContext>
    {
        public AnsatteController(SkoleContext db, ILogger<AnsatteController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnsatDto>>> GetAll()
            => await ProjectListAsync<Ansat, AnsatDto>(_db.Ansatte);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<AnsatDto>> Get(int id)
            => await GetByIdAsync<Ansat, AnsatDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AnsatDto>>> Search(string navn)
            => await GetFilteredAsync<Ansat, AnsatDto>(s => s.Navn.Contains(navn));

        // 4. Opret (Create)
        [HttpPost("CreateAnsat")]
        public async Task<ActionResult<AnsatDto>> CreateAnsat(AnsatCreateDto dto)
            => await CreateAsync<Ansat, AnsatCreateDto, AnsatDto>(dto, nameof(Get));

        
        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateAnsat/{id}")]
        public async Task<IActionResult> UpdateAnsat(int id, AnsatUpdateDto dto)
        {
            // Vi bruger den avancerede base-metode til at synkronisere 'Fag'
            // s.Fag er navnet på listen i din Student-entitet
            // dto.FagIds er listen med ID'er fra din DTO
            // f.FagId er navnet på nøglen i Fag-entiteten
            return await UpdateAsync<Ansat, AnsatUpdateDto>(
                id,
                dto);
        }


        // 6. Slet (Delete)
        [HttpDelete("DeleteSimpleObject/{id}")]
        public async Task<IActionResult> DeleteSimpleObject(int id)
            => await DeleteAsync<Ansat>(id);
    }
}
