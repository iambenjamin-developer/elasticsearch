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


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryStringParameters queryString)
        {
            var countResponse = await _elasticClient.CountAsync<Product>();
            var totalCount = countResponse.Count;

            var totalPages = (int)Math.Ceiling(totalCount / (double)queryString.PageSize);

            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                        .Query(q => q.MatchAll())
                                        .From((queryString.PageNumber - 1) * queryString.PageSize)
                                        .Size(queryString.PageSize));

            var entities = searchResponse.Documents;

            var result = entities?.ToList();



            var response = new
            {
                CurrentPage = queryString.PageNumber,
                PageSize = queryString.PageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasPreviousPage = queryString.PageNumber > 1,
                HasNextPage = queryString.PageNumber < totalPages,
                Items = result
            };
            return Ok(response);
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


        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] QueryStringParameters queryString)
        {
            var countResponse = await _elasticClient.CountAsync<Product>();
            var totalCount = countResponse.Count;

            var totalPages = (int)Math.Ceiling(totalCount / (double)queryString.PageSize);

            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                        .Query(q => q.MatchAll())
                                        .From((queryString.PageNumber - 1) * queryString.PageSize)
                                        .Size(queryString.PageSize)
                                        .Sort(x => x.Descending(y => y.Id))
                                        );

            var entities = searchResponse.Documents;

            var result = entities?.ToList();



            var response = new
            {
                CurrentPage = queryString.PageNumber,
                PageSize = queryString.PageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasPreviousPage = queryString.PageNumber > 1,
                HasNextPage = queryString.PageNumber < totalPages,
                Items = result
            };
            return Ok(response);
        }

        //public async Task<PagedItems<Product>> FindAsync(string searchTerm, int skip, int take)
        //{
           
        //    var response = await _elasticClient.SearchAsync<Product>(s => s
        //        .Index("")
        //        .From(skip).Size(take)
        //        .Query(q => q.MultiMatch(m => m.Fields(f => f
        //            .Field(u => u.Email)
        //            .Field(u => u.FirstName)
        //            .Field(u => u.LastName))
        //            .Query(searchTerm)))
        //        .Sort(q => q.Ascending(u => u.Email.Suffix("keyword"))));
        //    var count = await client.CountAsync<Product>(s => s.Index(_elasticConfiguration.GetIndex()));
        //    return new PagedItems<Product> { Items = response.Documents.ToArray(), Total = count.Count };
        //}

    }
}
