using API.Models;
using API.Models.Tests;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _TestController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;
        public static string PattyIndex = "atenea-tv-patty";

        public _TestController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestBody request)
        {
            var searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
                                       .Query(q => q.MatchAll())
                                       .Size(request.Size)
                                       .Scroll("1m"));

            var patties = searchResponse.Documents;

            var result = patties?.ToList();

            return Ok(result);
        }


        [HttpPost("filter-by-name")]
        public async Task<IActionResult> ByKeywords([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(x => x.Query(q => q
                                                   .Bool(bq => bq
                                                   .Filter(fq => fq.Terms(t => t.Field(f => f.Name).Terms(request.KeyWords))
                                                         //fq => fq.Terms(t => t.Field(f => f.Color).Terms(colorList))
                                                           ))));

            return Ok(searchResponse.Documents);
        }

        [HttpPost("filter-by-two-properties")]
        public async Task<IActionResult> ByTwoProperties([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(x => x.Query(q => q
                                                   .Bool(bq => bq
                                                   .Filter(fq => fq.Terms(t => t.Field(f => f.Name).Terms(request.KeyWords)),
                                                           fq => fq.Terms(t => t.Field(f => f.Stock).Terms(request.Quantity))
                                                           ))));

            return Ok(searchResponse.Documents);
        }
    }

}

