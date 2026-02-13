using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoldController : MyMapsterBaseController<HoldController, SkoleContext>
    {
        public HoldController(SkoleContext db, ILogger<HoldController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoldDto>>> GetAll()
            => await ProjectListAsync<Hold, HoldDto>(_db.Hold);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<HoldDto>> Get(int id)
            => await GetByIdAsync<Hold, HoldDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find studerende med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HoldDto>>> Search(string navn)
            => await GetFilteredAsync<Hold, HoldDto>(h => h.HoldNavn.Contains(navn));

        // 4. Opret (Create)
        [HttpPost("CreateHold")]
        public async Task<ActionResult<HoldDto>> CreateHold(HoldCreateDto dto)
            => await CreateAsync<Hold, HoldCreateDto, HoldDto>(dto, nameof(Get));


        // 5. Opdater (Update) - Inkluderer håndtering af mange-til-mange relation til Fag
        [HttpPut("UpdateHold/{id}")]
        public async Task<IActionResult> UpdateAnsat(int id, HoldUpdateDto dto)
        {
            // Vi bruger den avancerede base-metode til at synkronisere 'Fag'
            // s.Fag er navnet på listen i din Student-entitet
            // dto.FagIds er listen med ID'er fra din DTO
            // f.FagId er navnet på nøglen i Fag-entiteten
            return await UpdateAsync<Hold, HoldUpdateDto>(
                id,
                dto);
        }


        // 6. Slet (Delete)
        [HttpDelete("DeleteSimpleObject/{id}")]
        public async Task<IActionResult> DeleteSimpleObject(int id)
            => await DeleteAsync<Hold>(id);
    }
}
