using API.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PattiesController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        public static string PattyIndex = "atenea-tv-patty";

        public PattiesController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        // GET: api/<PattiesController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var searchResponse = _elasticClient.Search<Patty>(s => s
                                .Index(PattyIndex));

            var patties = searchResponse.Documents;

            var result = patties?.ToList();

            return Ok(result);
        }


        // GET api/<PattiesController>/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = _elasticClient.Get<Patty>(id);

            var response = result.Source;

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }


        // POST api/<PattiesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePatty request)
        {
            var random = new Random();
            long guid = random.Next(99, 999);

            var patty = new Patty()
            {
                Id = guid,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = DateTime.Now
            };

            var response = _elasticClient.IndexDocument(patty);

            if (response.IsValid)
            {
                return Created("NewPatty", patty);
            }
            else
            {
                return BadRequest();
            }
        }


        // PUT api/<PattiesController>/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdatePatty request)
        {
            var patty = new Patty()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = request.DateOfElaboration
            };

            var response = _elasticClient.Update<Patty, Patty>(patty.Id, d => d
                            .Index(PattyIndex)
                            .Doc(patty));

            if (response.IsValid)
            {
                return Ok(patty);
            }
            else
            {
                return BadRequest();
            }
        }


        // DELETE api/<PattiesController>/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = _elasticClient.Delete<Patty>(id, d => d
                            .Index(PattyIndex));

            if (response.IsValid)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
