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

        public PattiesController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        // GET: api/<PattiesController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var searchResponse = _elasticClient.Search<Patty>(s => s
                                .Index("atenea-tv-patty"));

            var people = searchResponse.Documents;

            var result = people?.ToList();

            return Ok(result);
        }

        // GET api/<PattiesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = _elasticClient.Get<Patty>(id);

            return Ok(result.Source);
        }

        // POST api/<PattiesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePatty request)
        {
            bool status;
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
                status = true;
            }
            else
            {
                status = false;
            }

            if (status)
            {
                return Created("NewPatty", patty);
            }
            else
            {
                return BadRequest();
            }

        }

        // PUT api/<PattiesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdatePatty request)
        {
            bool status;

            var patty = new Patty()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = DateTime.MinValue
            };

            var response = _elasticClient.Update<Patty, Patty>(patty.Id, d => d
                            .Index("atenea-tv-patty")
                            .Doc(patty));

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }


            if (status)
            {
                return Ok(patty);
            }
            else
            {
                return BadRequest();
            }

        }

        // DELETE api/<PattiesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool status;

            var response = _elasticClient.Delete<Patty>(id, d => d
                            .Index("atenea-tv-patty"));

            if (response.IsValid)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            status.ToString();

            return NoContent();
        }
    }
}
