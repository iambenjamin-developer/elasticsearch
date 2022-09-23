using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PattiesController : ControllerBase
    {
        // GET: api/<PattiesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        // GET api/<PattiesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            return Ok();
        }

        // POST api/<PattiesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            return Created("NewPatty", "");
        }

        // PUT api/<PattiesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        // DELETE api/<PattiesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return NoContent();
        }
    }
}
