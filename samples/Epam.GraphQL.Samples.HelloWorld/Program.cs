using System;
using System.Threading.Tasks;
using Epam.GraphQL;
using Epam.GraphQL.SystemTextJson;

namespace Epam.GraphQL.Samples.HelloWorld
{
    /// <summary>
    /// The GraphQL execution context.
    /// Instance of this class is accessible during field resolving, so it is a good place for holding necessary stuff for data querying.
    /// </summary>
    public class GraphQLExecutionContext
    {
        public string? CurrentName { get; set; }
    }

    /// <summary>
    /// In this class we should define a shape of the query type and how it retrieves data by overriding OnConfigure method.
    /// This class should be derived from Query<TExecutionContext> with a concrete execution context as a generic type parameter.
    /// </summary>
    public class GraphQLQuery : Query<GraphQLExecutionContext>
    {
        protected override void OnConfigure()
        {
            Field("hello") // We define field with name "hello"
                .Resolve(context => $"Hello, {context.CurrentName}!"); // ... and retrive field value via field resolver which has one parameter of type GraphQLExecutionContext.
        }
    }


    class Program
    {
        static async Task Main()
        {
            // Instantiate SchemaExecuter with type parameters of previously defined GraphQLQuery and GraphQLExecutionContext
            var schemaExecuter = new SchemaExecuter<GraphQLQuery, GraphQLExecutionContext>();

            var executionContext = new GraphQLExecutionContext
            {
                CurrentName = "world"
            };

            // Define options of GraphQL query execution. A minimal set of options is a query and an execution context

            var result = await schemaExecuter.ExecuteAsync(optionsBuilder => optionsBuilder
                .WithExecutionContext(executionContext) // Specify an instance of execution context to work with
                .Query(@"
                    query {
                        hello 
                    }"))
                .ToStringAsync(indent: true) // Convert ExecutionResult to string using extension method from Epam.SystemTextJson
                .ConfigureAwait(false);

            Console.WriteLine(schemaExecuter.Print());
            Console.WriteLine(result);
        }
    }
}
