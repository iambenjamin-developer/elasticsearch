using API.Models;
using API.Models.Tests;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
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

        [HttpPost("menor-que")]
        public async Task<IActionResult> MenorQue([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
            .Query(q => q
                .Bool(b => b
                    //I'm using date range in filter context as I don't want elasticsearch
                    //to calculate score for each document found,
                    //should be faster and likely it will be cached
                    .Filter(f =>
                        f.DateRange(dt => dt
                            .Field(field => field.DateOfElaboration)
                            .LessThan(request.DateTo)
                            )))));

            return Ok(searchResponse.Documents);
        }

        [HttpPost("menor-o-igual-que")]
        public async Task<IActionResult> MenorOIgualQue([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
            .Query(q => q
                .Bool(b => b
                    //I'm using date range in filter context as I don't want elasticsearch
                    //to calculate score for each document found,
                    //should be faster and likely it will be cached
                    .Filter(f =>
                        f.DateRange(dt => dt
                            .Field(field => field.DateOfElaboration)
                            .LessThanOrEquals(request.DateTo)
                            )))));

            return Ok(searchResponse.Documents);
        }

        [HttpPost("mayor-que")]
        public async Task<IActionResult> MayorQue([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
            .Query(q => q
                .Bool(b => b
                    //I'm using date range in filter context as I don't want elasticsearch
                    //to calculate score for each document found,
                    //should be faster and likely it will be cached
                    .Filter(f =>
                        f.DateRange(dt => dt
                            .Field(field => field.DateOfElaboration)
                            .GreaterThan(request.DateFrom)
                            )))));

            return Ok(searchResponse.Documents);
        }

        [HttpPost("mayor-o-igual-que")]
        public async Task<IActionResult> MayorOIgualQue([FromBody] RequestBody request)
        {
            ISearchResponse<Patty> searchResponse = await _elasticClient.SearchAsync<Patty>(s => s
            .Query(q => q
                .Bool(b => b
                    //I'm using date range in filter context as I don't want elasticsearch
                    //to calculate score for each document found,
                    //should be faster and likely it will be cached
                    .Filter(f =>
                        f.DateRange(dt => dt
                            .Field(field => field.DateOfElaboration)
                            .GreaterThanOrEquals(request.DateFrom)
                            )))));

            return Ok(searchResponse.Documents);
        }
    }

}

