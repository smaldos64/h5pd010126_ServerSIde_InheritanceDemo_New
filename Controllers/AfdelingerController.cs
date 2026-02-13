using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AfdelingController : MyMapsterBaseController<AfdelingController, SkoleContext>
    {
        public AfdelingController(SkoleContext db, ILogger<AfdelingController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AfdelingDto>>> GetAll()
            => await ProjectListAsync<Afdeling, AfdelingDto>(_db.Afdelinger);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<AfdelingDto>> Get(int id)
            => await GetByIdAsync<Afdeling, AfdelingDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AfdelingDto>>> Search(string navn)
            => await GetFilteredAsync<Afdeling, AfdelingDto>(a => a.AfdelingNavn.Contains(navn));

        // 4. Opret (Create)
        [HttpPost("CreateAfdeling")]
        public async Task<ActionResult<AfdelingDto>> CreateAfdeling(AfdelingCreateDto dto)
            => await CreateAsync<Afdeling, AfdelingCreateDto, AfdelingDto>(dto, nameof(Get));


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateAfdeling/{id}")]
        public async Task<IActionResult> UpdateAnsat(int id, AfdelingUpdateDto dto)
        {
            // Vi bruger den avancerede base-metode til at synkronisere 'Fag'
            // s.Fag er navnet på listen i din Student-entitet
            // dto.FagIds er listen med ID'er fra din DTO
            // f.FagId er navnet på nøglen i Fag-entiteten
            return await UpdateAsync<Afdeling, AfdelingUpdateDto>(
                id,
                dto);
        }


        // 6. Slet (Delete)
        [HttpDelete("DeleteSimpleObject/{id}")]
        public async Task<IActionResult> DeleteSimpleObject(int id)
            => await DeleteAsync<Afdeling>(id);
    }
}
