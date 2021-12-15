using System;
using System.Threading.Tasks;
using Epam.GraphQL;
using Epam.GraphQL.Samples.Data;
using Epam.GraphQL.SystemTextJson;

namespace Epam.GraphQL.Samples.EF.Automapping
{
    class Program
    {
        static async Task Main()
        {
            using var ctx = new GraphQLDbContext();

            var schemaExecuter = new SchemaExecuter<GraphQLQuery, GraphQLExecutionContext>();
            var executionContext = new GraphQLExecutionContext
            {
                DbContext = ctx
            };

            var result = await schemaExecuter.ExecuteAsync(optionsBuilder => optionsBuilder
                .WithExecutionContext(executionContext)
                .WithDbContext(ctx) //
                .Query(@"
                    query {
                        departments {
                            id
                            name
                        }
                    }"))
                .ToStringAsync(indent: true)
                .ConfigureAwait(false);

            Console.WriteLine(schemaExecuter.Print());
            Console.WriteLine(result);
        }
    }
}
