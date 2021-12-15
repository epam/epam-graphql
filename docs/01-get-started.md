# Get Started
## Installation

```cmd
dotnet add package Epam.GraphQL
```

It is worth to install the other packages related to `Epam.GraphQL` (if needed):

| Package                          | Description                                         |
|----------------------------------|-----------------------------------------------------|
| Epam.GraphQL.SystemTextJson      | Helpers for SystemTextJson-based json serialization |
| Epam.GraphQL.NewtonsoftJson      | Helpers for NewtonsoftJson-based json serialization |
| Epam.GraphQL.EntityFrameworkCore | EntityFrameworkCore bindings                        |
| Epam.GraphQL.MiniProfiler        | [MiniProfiler](https://miniprofiler.com/) bindings  |

## Hello, world!

This is a "Hello, world!" example of `Epam.GraphQL` usage. In addition to `Epam.GraphQL`, you have to add a reference to  `Epam.GraphQL.SystemTextJson` (or `Epam.GraphQL.NewtonsoftJson`)  for result serialization.

```csharp
using System;
using System.Threading.Tasks;
using Epam.GraphQL;
using Epam.GraphQL.SystemTextJson;

namespace HelloWorld
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

            Console.WriteLine(result);
        }
    }
}
```

Output:

```json
{
  "data": {
    "hello": "Hello, world!"
  }
}
```

GraphQL Schema:

```graphql
schema {
  query: GraphQLQuery
}

type GraphQLQuery {
  hello: String
}
```

The full working example can be found [here](../samples/Epam.GraphQL.Samples.HelloWorld/).

