using System;
using System.Threading.Tasks;
using Epam.GraphQL.Samples.Data;
using Epam.GraphQL.SystemTextJson;

namespace Epam.GraphQL.Samples.EF.Mutation
{
    class Program
    {
        static async Task Main()
        {
            using var ctx = new GraphQLDbContext();

            var schemaExecuter = new SchemaExecuter<GraphQLQuery, GraphQLMutation, GraphQLExecutionContext>();
            var executionContext = new GraphQLExecutionContext
            {
                DbContext = ctx
            };

            var result = await schemaExecuter.ExecuteAsync(optionsBuilder => optionsBuilder
                .WithExecutionContext(executionContext)
                .WithDbContext(ctx) //
                .Query(@"
                    mutation {
                        submit(payload: {
                            departments: [{
                                id: 1
                                modifiedAt: null
                            }]
                        }) {
                            departments {
                                id
                                payload {
                                    id
                                    name
                                    modifiedAt
                                }
                            }
                        }
                    }"))
                .ToStringAsync(indent: true)
                .ConfigureAwait(false);

            Console.WriteLine(schemaExecuter.Print());
            Console.WriteLine(result);
        }
    }
}
