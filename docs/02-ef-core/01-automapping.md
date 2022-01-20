# Entity Framework Core Usage Basics: Automapping

[DbContext](../02-ef-schema.md#dbcontext) and [models](../02-ef-schema.md#models) models for this example are defined in the [Epam.GraphQL.Samples.Data project](../../samples/Epam.GraphQL.Samples.Data/).

Suppose we have to expose list of departments via GraphQL.

## Execution Context
First of all, we have to define an execution context. It is a class, which is accessible during graphql fields resolution process. Usually it contains all stuff that is needed for fields querying/calculation. In our case, we should have an access to [Entity Framework DbContext](../02-ef-schema#dbcontext):

```csharp
public class GraphQLExecutionContext
{
    /// <summary>
    /// DbContext should be declared here, because you need access to it for data querying.
    /// </summary>
    public GraphQLDbContext DbContext { get; set; }
}
```

## GraphQL Query
Next, we need another class, which has to be inherited from `Epam.GraphQL.Query` with a generic parameter of execution context type (`GraphQLExecutionContext`), defined earlier. It should contain configurations for all GraphQL query root fields in `OnConfigure` method. Configuration of a field should consists of two things at least:
* Field name
* How data should be fetched for this field

```csharp
public class GraphQLQuery : Query<GraphQLExecutionContext>
{
    protected override void OnConfigure()
    {
        // Populate all data from Departments data set. A model will be mapped to GraphQL type automatically.
        Field("departments")
            .FromIQueryable(context => context.DbContext.Departments);
    }
}
```

In the `GraphQLQuery.OnConfigure` method above, one root field is defined:
* It has name `departments`:
  ```csharp
  Field("departments")
  ```
* It should return data from field `Departmnets` of Entity Framework DbContext:
  ```csharp
  .FromIQueryable(context => context.DbContext.Departments);
  ```

## Query
```csharp
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

        Console.WriteLine(result);
    }
}
```

Given [the department table data](../02-ef-schema.md#Department%20Data), the code above produces the following output:

```json
{
  "data": {
    "departments": [
      {
        "id": 1,
        "name": "Alpha"
      },
      {
        "id": 2,
        "name": "Beta"
      },
      {
        "id": 3,
        "name": "Gamma"
      }
    ]
  }
}
```

SQL:

```sql
SELECT "d"."Id" AS "id", "d"."Name" AS "name"
FROM "Departments" AS "d"
```

The following GraphQL schema is created:

```graphql
schema {
  query: GraphQLQuery
}

type Department {
  id: Int!
  name: String
  parentId: Int
  parent: Department
  children: [Department]
}

type GraphQLQuery {
  departments: [Department]
}
```

The full working example can be found [here](../../samples/Epam.GraphQL.Samples.EF.Automapping/).