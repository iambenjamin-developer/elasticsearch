using Nest;
using System;
using System.Linq.Expressions;

namespace API.Extensions
{
    public static class ElasticClientExtensions
    {
        public static ISearchResponse<T> SearchWithMatch<T>(this IElasticClient client, Expression<Func<T, object>> field)
            where T : class =>
            client.Search<T>(s => s
                .From(0)
                .Size(10)
                .Index("project-microservice-products*")
                .Query(q => q
                    .Match(m => m
                        .Field(field)
                        .Query("salesinvoice")
                    )
                )
            );
    }
}
