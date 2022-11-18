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

    }
}
