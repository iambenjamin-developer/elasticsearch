﻿using API.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public static string ProductIndex = "appname-servicename-product-index";
        private readonly IElasticClient _elasticClient;


        public ProductsController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int size = 10)
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                        .Query(q => q.MatchAll())
                                        .Size(size)
                                        .Scroll("1m"));

            var entities = searchResponse.Documents;

            var result = entities?.ToList();

            return Ok(result);
        }


        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _elasticClient.GetAsync<Product>(id);

            var entity = response.Source;

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProduct request)
        {
            var random = new Random();
            long id = random.Next(99, 999999);

            var entity = new Product()
            {
                Id = id,
                Name = request.Name,
                Stock = request.Stock,
                ExpirationDate = DateTime.Now.AddDays(90),
                IsActive = true,
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


        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProduct request)
        {
            var getResponse = await _elasticClient.GetAsync<Product>(id);

            var entity = getResponse.Source;

            if (entity == null)
            {
                return NotFound();
            }

            //entity.Name = request.Name;
            //entity.Stock = request.Stock;

            var updateResponse = await _elasticClient.UpdateAsync<Product>(entity.Id, u => u
                                               .Index(ProductIndex)
                                               .Doc(entity));

            if (updateResponse.IsValid)
            {
                return Ok(entity);
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var getResponse = await _elasticClient.GetAsync<Product>(id);
            var entity = getResponse.Source;

            if (entity == null)
            {
                return NotFound();
            }

            var deleteResponse = await _elasticClient.DeleteAsync<Product>(entity);

            if (deleteResponse.IsValid)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }


        }
    }
}
