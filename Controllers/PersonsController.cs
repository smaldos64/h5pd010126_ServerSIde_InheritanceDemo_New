using InheritanceDemo.Data;
using InheritanceDemo.DTOs;
using InheritanceDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InheritanceDemo.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : MyMapsterBaseController<PersonsController, SkoleContext>
    {
        public PersonsController(SkoleContext db, ILogger<PersonsController> logger)
            : base(db, logger)
        {
        }

        // 1. Hent alle (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll()
            => await ProjectListAsync<Person, PersonDto>(_db.Personer);

        // 2. Hent én baseret på Primary Key (den skudsikre GetByIdAsync)
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDto>> Get(int id)
            => await GetByIdAsync<Person, PersonDto>(id);

        // 3. Søg efter en specificeret query (GetFiltered)
        // Eksempel: Find lærere med et specifikt navn eller over en vis alder
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> Search(string navn)
            => await GetFilteredAsync<Person, PersonDto>(t => t.Navn.Contains(navn));
    }
}
