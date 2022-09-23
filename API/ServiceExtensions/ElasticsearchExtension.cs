using API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace API.ServiceExtensions
{
    public static class ElasticsearchExtensions
    {
        public static string PattyIndex = "atenea-tv-patty";
        public static string ProductIndex = "appname-servicename-product-index";

        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var user = configuration["elasticsearch:user"];
            var password = configuration["elasticsearch:password"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                .BasicAuthentication(user, password);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
            CreatePattyIndex(client, PattyIndex);
            CreateProductIndex(client, ProductIndex);
        }

        #region Private Methods

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<Patty>(t => t.IndexName(PattyIndex));
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var response = client.Indices.Create(indexName,
                         index => index.Map<Man>(
                             x => x.AutoMap()
                         ));
        }

        private static void CreatePattyIndex(IElasticClient client, string indexName)
        {
            var response = client.Indices.Create(indexName,
                index => index.Map<Patty>(
                    x => x.AutoMap()
                ));
        }

        private static void CreateProductIndex(IElasticClient client, string indexName)
        {
            var response = client.Indices.Create(indexName,
                index => index.Map<Product>(
                    x => x.AutoMap()
                ));
        }

        #endregion
    }
}