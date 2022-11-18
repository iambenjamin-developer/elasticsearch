using API.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsyncPattiesController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        public static string PattyIndex = "atenea-tv-patty";

        public AsyncPattiesController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        // GET: api/<PattiesController>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int size = 10)
        {
            var searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
                                        .Query(q => q.MatchAll())
                                        .Size(size)
                                        .Scroll("1m"));

            var patties = searchResponse.Documents;

            var result = patties?.ToList();

            return Ok(result);
        }


        // GET api/<PattiesController>/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _elasticClient.GetAsync<Patty>(id);

            var response = result.Source;

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        // GET api/<PattiesController>/5
        [HttpGet("V2{id:long}")]
        public async Task<IActionResult> GetV2(long id)
        {
            var result = await _elasticClient.GetAsync<Patty>(id, x => x.Index(PattyIndex));

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

            var response = await _elasticClient.IndexDocumentAsync(patty);

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
            var entity = new Patty()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = request.DateOfElaboration
            };

            var response = await _elasticClient.UpdateAsync<Patty>(entity.Id, u => u
                                               .Index(PattyIndex)
                                               .Doc(entity));

            if (response.IsValid)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest();
            }
        }


        // PUT api/<PattiesController>/5
        [HttpPut("V2{id:long}")]
        public async Task<IActionResult> PutV2(long id, [FromBody] UpdatePatty request)
        {
            var entity = new Patty()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = request.DateOfElaboration
            };

            var response = await _elasticClient.UpdateAsync<Patty>(entity.Id, u => u
                                               .Index(PattyIndex)
                                               .Doc(entity));

            if (response.IsValid)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT api/<PattiesController>/5
        [HttpPut("V3{id:long}")]
        public async Task<IActionResult> PutV3(long id, [FromBody] UpdatePatty request)
        {
            var entity = new Patty()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                DateOfElaboration = request.DateOfElaboration
            };

            var response = await _elasticClient.UpdateAsync<object>(entity.Id, u => u
                                               .Index(PattyIndex)
                                               .Doc(new { AnonymousProperty = DateTime.Now.ToLongDateString() }));

            if (response.IsValid)
            {
                return Ok(entity);
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
            var response = await _elasticClient.DeleteAsync<Patty>(id, d => d
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

        // DELETE api/<PattiesController>/5
        [HttpDelete("V2{id:long}")]
        public async Task<IActionResult> DeleteV2(long id)
        {
            var response = await _elasticClient.DeleteAsync<Patty>(id);

            if (response.IsValid)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/<PattiesController>/5
        [HttpDelete("V3{id:long}")]
        public async Task<IActionResult> DeleteV3(long id)
        {
            var getResponse = await _elasticClient.GetAsync<Patty>(id);
            var entity = getResponse.Source;

            if (entity == null)
            {
                return NotFound();
            }

            var deleteResponse = await _elasticClient.DeleteAsync<Patty>(entity.Id);

            return NoContent();
        }

        // DELETE api/<PattiesController>/5
        [HttpDelete("V4{id:long}")]
        public async Task<IActionResult> DeleteV4(long id)
        {
            var getResponse = await _elasticClient.GetAsync<Patty>(id);
            var entity = getResponse.Source;

            if (entity == null)
            {
                return NotFound();
            }

            var deleteResponse = await _elasticClient.DeleteAsync<Patty>(entity);

            return NoContent();
        }


        [HttpGet("Count")]
        public async Task<IActionResult> Count([FromQuery] int size = 10)
        {
            var total = await _elasticClient.CountAsync<Patty>();
            var totalPages = total.Count > 0 ? total.Count / size : 0;

            //var searchResponse = _elasticClient.Scroll<Patty>("1m", "ScrollId98735");
            var result = new
            {
                Total = total.Count,
                TotalPages = totalPages,
                //SearchResponse = searchResponse
            };
            //var json = JsonSerializer.Serialize(result);

            return Ok(result);
        }
    }
}
