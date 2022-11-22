using API.Commons;
using API.Extensions;
using API.Models.Commons;
using API.Models.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _ProductsController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;
        private readonly IConfiguration _configuration;
        public static string ProductIndex = "project-microservice-products";

        public _ProductsController(IElasticClient elasticClient, IConfiguration configuration)
        {
            _elasticClient = elasticClient;
            _configuration = configuration;
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

        [HttpGet("test")]
        public async Task<IActionResult> Test([FromQuery] QueryStringParameters queryString)
        {

            var searchResponse = _elasticClient.SearchWithMatch<Product>(f => f.Stock);

            var entities = searchResponse.Documents;

            var result = entities?.ToList();

            return Ok(result);
        }

        [HttpGet("_avatel")]
        public async Task<IActionResult> Avatel([FromQuery] QueryStringParameters query)
        {
            /*
            var res1 = elasticClient.Search<HistoryRecord>(s => s.Index("apm-7.10.1-transaction")
            .From(0)
            .Size(100)
            .Query( q => q
            .Term(t => t.RequestBody, "") || q
            .Term(t => t.ResponseBody, "") || q
            .Match(mq => mq.Field(f => f.RequestBody).Query(""))
            ));
            */

            var index = _configuration.GetValue<string>("ElasticApm:ProductIndex");

            var from = (query.PageNumber - 1) * query.PageSize;
            var size = query.PageSize;

            var searchDescriptor = new SearchDescriptor<Product>()
                    .Index(index)
                    .From(from)
                    .Size(size);

            SearchByProperties(query, ref searchDescriptor);

            var res = await _elasticClient.SearchAsync<Product>(searchDescriptor); // Usar el tipo dynamic para explorar el resultado completo que devuelve ES
            //var res1 = await elasticClient.SearchAsync<APMHistoryRecord>(searchDescriptor); // TODO: Usar implementaci?n con el tipo APMHistoryRecord
            List<Product> entities = new List<Product>();
            foreach (var row in res.Documents)
            {
                entities.Add(row);
                try
                {
                    entities.Add(row);
                }
                catch (Exception e)
                {
                    entities.Add(new Product()); // Si hay un error, inserta un elemento vac?o. Esto se hace para favorecer la detecci?n de errores.
                }
            }
            var resultadoPaginado = new PagedList<Product>(entities, (int)res.Total, query.PageNumber, query.PageSize);

            return new ContentResult { Content = resultadoPaginado.ToJson(), ContentType = "application/json", };
        }


        private void SearchByProperties(QueryStringParameters query, ref SearchDescriptor<Product> searchDescriptor)
        {
            searchDescriptor.Query(s => s.MatchAll()).Sort(x => x.Descending(y => y.Id));


        }

    }
}
