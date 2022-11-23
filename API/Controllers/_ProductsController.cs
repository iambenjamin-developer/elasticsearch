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
            /*
 https://docs.oracle.com/javase/8/docs/api/java/time/format/DateTimeFormatter.html

Formatter	 Description	                 Example
ISO_INSTANT  Date and Time of an Instant	'2011-12-03T10:15:30Z'
 */
            return Ok(DateTime.Now.AddHours(9).ToString("yyyy-MM-ddTHH:mm:ssZ"));
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

            List<Product> entities = new List<Product>();
            foreach (var row in res.Documents)
            {
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
            //searchDescriptor.Query(s => s.MatchAll()).Sort(x => x.Descending(y => y.Id));


            string id = query.FilterById?.Trim();
            string guid = query.FilterByGuid?.Trim();
            string name = query.FilterByName?.Trim();
            string stock = query.FilterByStock?.Trim();
            string order = query.Order?.Trim();
            string dateOfExpiration = query.FilterByDateOfExpiration?.Trim();
            string dateFrom = query.FilterDateFrom?.Trim();
            string dateTo = query.FilterDateTo?.Trim();

            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(stock) && string.IsNullOrWhiteSpace(guid) && string.IsNullOrWhiteSpace(order) && string.IsNullOrWhiteSpace(dateOfExpiration) && string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo))
            {
                searchDescriptor.Query(s => s.MatchAll()).Sort(x => x.Descending(y => y.Id));

                return;
            }

            //Filtrar por ID
            if (!string.IsNullOrWhiteSpace(id))
            {
                searchDescriptor.Query(q => q.Match(m => m.Field(x => x.Id).Query(id)));
            }

            //Filtrar por GUID
            if (!string.IsNullOrWhiteSpace(guid))
            {
                searchDescriptor.Query(q => q.Match(m => m.Field(x => x.Guid).Query(guid)));
            }

            //Filtrar por nombre
            if (!string.IsNullOrWhiteSpace(name))
            {
                searchDescriptor.Query(q => q.Match(m => m.Field(x => x.Name).Query(name)));
            }

            //Filtrar por Stock
            if (!string.IsNullOrWhiteSpace(stock))
            {
                searchDescriptor.Query(q => q.Match(m => m.Field(x => x.Stock).Query(stock)));
            }

            //Filtrar por Fecha de vencimiento.Formato:
            //    2022-11-30T08:47:53.7320000Z
            //
            //2022 - 01 - 01T00: 00:00.0000000Z

            //2022 - 12 - 31T23: 59:59.9999999Z

            /*
             https://docs.oracle.com/javase/8/docs/api/java/time/format/DateTimeFormatter.html

Formatter	 Description	                 Example
ISO_INSTANT  Date and Time of an Instant	'2011-12-03T10:15:30Z'
             */


            if (!string.IsNullOrWhiteSpace(dateOfExpiration))
            {
                string start = dateOfExpiration + "T00:00:00.0000000Z";
                string end = dateOfExpiration + "T23:59:59.9999999Z";

                searchDescriptor.Query(q => q.DateRange(dt => dt.Field(f => f.DateOfExpiration)
                                                          .GreaterThanOrEquals(start)
                                                          .LessThanOrEquals(end)));
            }

            //Desde y hasta 
            if (!string.IsNullOrWhiteSpace(dateFrom) && !string.IsNullOrWhiteSpace(dateTo))
            {
                searchDescriptor.Query(q => q.DateRange(dt => dt.Field(f => f.DateOfExpiration)
                                                                .GreaterThanOrEquals(dateFrom)
                                                                .LessThanOrEquals(dateTo)));
            }

            //Solo desde
            if (!string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo))
            {
                searchDescriptor.Query(q => q.DateRange(dt => dt.Field(f => f.DateOfExpiration).GreaterThanOrEquals(dateFrom)));
            }

            //Solo hasta
            if (!string.IsNullOrWhiteSpace(dateTo) && string.IsNullOrWhiteSpace(dateFrom))
            {
                searchDescriptor.Query(q => q.DateRange(dt => dt.Field(f => f.DateOfExpiration).LessThanOrEquals(dateTo)));
            }


            //Ordenar
            if (!string.IsNullOrWhiteSpace(order))
            {
                if (query.Order == "asc")
                {
                    searchDescriptor.Sort(x => x.Ascending(y => y.DateOfExpiration));
                }
                else
                {
                    searchDescriptor.Sort(x => x.Descending(y => y.DateOfExpiration));
                }
            }


            //searchDescriptor.Query(q => q.DateRange(dt => dt.Field(f => f.DateOfExpiration).GreaterThanOrEquals(dateOfExpiration)));

            /*
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
             */
            //string search = query.SearchQuery?.Trim();
            //string user = query.FilterUser?.Trim();
            //string type = query.FilterType;
            //int status = query.FilterStatus;
            //int operation = query.FilterOperation;
            //string dateFrom = query.DateFrom?.Trim();
            //string dateTo = query.DateTo?.Trim();
            //string operatorId = query.OperatorId != null && query.OperatorId > 0 ? query.OperatorId.ToString() : string.Empty;
            //string typeQuery, operationQuery, statusQuery;
            //typeQuery = operationQuery = statusQuery = String.Empty;


            //if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(user) && type == "0" && status == 0 && operation == 0 && string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo) && string.IsNullOrWhiteSpace(operatorId))
            //{

            //    searchDescriptor.Query(q => q.MatchAll()).Sort(s => s.Descending(f => f.Id));
            //}
            //else
            //{

            //    var entityQuery = new QueryStringQuery();
            //    if (!string.IsNullOrEmpty(search))
            //    {
            //        entityQuery.Query = $"url.query:*{search}* OR labels.message:*{search}* OR labels.operatorId:*{search}* OR labels.operatorName:*{search}* OR http.request.body.original:*{search}*";
            //    }

            //    // Filtro de b?squeda full text
            //    var searchQuery = new WildcardQuery();

            //    if (!String.IsNullOrWhiteSpace(type) && type != "0")
            //    {
            //        searchQuery.Value = $"*{type}*";
            //        searchQuery.CaseInsensitive = true;
            //        searchQuery.Field = "labels.message";
            //        searchQuery.Boost = 1.1;
            //    }
            //    var userQuery = new QueryStringQuery();

            //    if (!String.IsNullOrWhiteSpace(user))
            //    {
            //        userQuery.Fields = "user.name";
            //        userQuery.Fields.And("user.email");
            //        userQuery.Query = "*" + user + "*";
            //        userQuery.AnalyzeWildcard = true;
            //    }

            //    //Filtrar por OperatorId
            //    var operatorQuery = new QueryStringQuery();

            //    if (!String.IsNullOrWhiteSpace(operatorId))
            //    {
            //        operatorQuery.Fields = "labels.operatorId";
            //        operatorQuery.Fields.And("labels.operatorId");
            //        operatorQuery.Query = operatorId;
            //        operatorQuery.AnalyzeWildcard = true;
            //    }

            //    // Filtro de fecha
            //    var dateRangeQuery = new DateRangeQuery();
            //    dateRangeQuery.Field = "@timestamp";
            //    DateMath dmFrom = DateMath.Anchored(new DateTime(2020, 01, 01));
            //    DateMath dmTo = DateMath.Anchored(new DateTime().AddDays(1));
            //    if (!string.IsNullOrWhiteSpace(dateFrom))
            //    {
            //        if (DateTime.TryParse(dateFrom, out DateTime from))
            //        {
            //            dmFrom = DateMath.Anchored(from);
            //            dateRangeQuery.GreaterThanOrEqualTo = dmFrom;
            //        }
            //    }

            //    if (!string.IsNullOrWhiteSpace(dateTo))
            //    {
            //        if (DateTime.TryParse(dateTo, out DateTime to))
            //        {
            //            dmTo = DateMath.Anchored(to.AddDays(1));
            //            dateRangeQuery.LessThanOrEqualTo = dmTo;
            //        }
            //    }

            //    // Descriptor de b?squeda
            //    searchDescriptor.Query(q => entityQuery && searchQuery && userQuery && dateRangeQuery && operatorQuery);

            //    // Filtro de ordenaci?n
            //    searchDescriptor.Sort(s => s.Descending(f => f.Id));

            //    /*
            //    searchDescriptor.Query(q => 
            //        q.QueryString(qs => qs.Query(result))
            //        && q.DateRange( dr => dr.Field("@timestamp").GreaterThanOrEquals(dmFrom).LessThanOrEquals(dmTo))
            //    ).Sort(s => s.Descending(f => f.timestamp.us));
            //    */
        }
    }

}

