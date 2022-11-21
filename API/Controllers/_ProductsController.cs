using API.Models;
using API.Models.Commons;
using API.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _ProductsController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;
        public static string ProductIndex = "project-microservice-products";

        public _ProductsController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count([FromQuery] int size = 10)
        {
            var total = await _elasticClient.CountAsync<Product>();
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryStringParameters queryString)
        {
            var countResponse = await _elasticClient.CountAsync<Product>();
            var totalPages = (int)Math.Ceiling(countResponse.Count / (double)queryString.PageSize);

            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                        .Query(q => q.MatchAll())
                                        .From((queryString.PageNumber - 1) * queryString.PageSize)
                                        .Size(queryString.PageSize)
         
                                        );

            var entities = searchResponse.Documents;

            var result = entities?.ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto request)
        {
            var random = new Random();
            long randomId = random.Next(99, 999);

            var entity = new Product()
            {
                Id = randomId,
                Guid = Guid.NewGuid().ToString(),
                Name = request.Name,
                Stock = request.Stock,
                DateOfExpiration = request.DateOfExpiration,
                UpdatedDate = DateTime.Now,
                DealerId = request.DealerId,
                Price = request.Price,
                Enabled = request.Enabled
            };

            var response = await _elasticClient.IndexDocumentAsync(entity);

            if (response.IsValid)
            {
                return Created("NewProduct", entity);
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
