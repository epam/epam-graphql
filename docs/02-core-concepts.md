# Core Concepts
If you are not familiar with GraphQL, the good place to start is [this resource](https://graphql.org/learn/). 

## Execution Context

An execution context is a class, which instance is accessible during GraphQL fields resolution process. Usually it contains all stuff that is needed for fields querying/calculation; e.g. if you need to query database via EF Core, you have to store [Entity Framework DbContext](00-samples-ef-schema.md#dbcontext) in an execution context's property:
```csharp
public class GraphQLExecutionContext
{
    /// <summary>
    /// DbContext should be declared here, because you need access to a data via EF Core.
    /// </summary>
    public GraphQLDbContext DbContext { get; set; }
}
```
There are no constraints on a class that implements execution context. It doesn't have either to be inherited from a particular class or to implement a particular interface.

## Query

The query class represents a GraphQL query. It has to be inherited from `Epam.GraphQL.Query<TExecutionContext>` class, provided by Epam.GraphQL, with a type parameter of [an execution context type](#execution-context). It should contain configurations for all GraphQL query root fields in `OnConfigure` method. Configuration of a field should consist of three things at least (implicitly or explicitly):
* Field name
* How data should be mapped to a GraphQL type
* How data should be fetched for this field

### Query Example

```csharp
public class GraphQLQuery : Query<GraphQLExecutionContext>
{
    protected override void OnConfigure()
    {
        // Populate all data from Continents data set. A model will be mapped to GraphQL type automatically.
        Field("continents")
           .FromIQueryable(context => context.DbContext.Continents);
    }
}
```

In the `GraphQLQuery.OnConfigure` method above, one root field is defined:
* It has explicit name `continents`:
  ```csharp
  Field("continents")
  ```
* [The type of `Continent`](00-samples-ef-schema.md#continent) model is [automatically (implicitly) mapped](#auto-mapping) to a GraphQL type.
* It returns all data from  `Continents` data set of Entity Framework DbContext (`context` argument of a delegate has `GraphQLExecutionContext` type passed as a type parameter to `Epam.GraphQL.Query` class, so that you can access to an instance of execution context and thus to all its properties):
  ```csharp
  .FromIQueryable(context => context.DbContext.Continents);
  ```

## CLR Types to GraphQL Types Mapping
### Auto Mapping

In the [example](#query-example) above, an expression `context.DbContext.Continents` for the query field `continents` is coerced to a GraphQL type automatically. The following rules are applied to a CLR type of expression `context.DbContext.Continents`:
1. Primitive CLR value types (`int`, `long`, `double`, `float`, `decimal`, `bool`) and some .NET Framework types (`DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`) are mapped to [GraphQL scalar types](https://graphql.org/learn/schema/#scalar-types) automatically. These types are mapped to [non-nullable GraphQL ones](https://graphql.org/learn/schema/#lists-and-non-null).
2. The `string` type is mapped to nullable GraphQL type `String`.
3. Nullable value types (e.g `int?`, `DateTime?`, `bool?`) are mapped to nullable GraphQL types (`Int`, `DateTime`, `Boolean`).
4. Enum types are mapped to [enumeration GraphQL types](https://graphql.org/learn/schema/#enumeration-types). Epam.GraphQL generates enumeration types on-the-fly. The names of values are converted to `CAPITAL_CASE`.
5. CLR types, which support `IEnumerable<T>` interface, where a type `T` is convertible to a GraphQL type `TGraphQLType`, are mapped to a [GraphQL list type](https://graphql.org/learn/schema/#lists-and-non-null) `[TGraphQLType]` (keep in mind that the type `[TGraphQLType]` is nullable GraphQL type). For example, `List<int>` is coerced to `[Int!]`.
6. CLR reference types are mapped to [GraphQL object types](https://graphql.org/learn/schema/#object-types-and-fields). Each property of CLR type is mapped recursively (using these rules) to a GraphQL object type field with the camel-cased name. There are a few restrictions for automatic conversion:
    * Only properties with getters are considered for conversion. Write-only properties are ignored.
    * [Indexers](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/) are ignored.
    * The type has to contain one readable field at least. This means, for instance, that the CLR `object` type cannot be converted to a GraphQL type because it does not contain properties.

Applying these rules to the [example](#query-example) leads to the following GraphQL schema (models `Continent`, `Country` and `City` can be found [here](00-samples-ef-schema.md#models)):

```graphql
type GraphQLQuery {
  continents: [Continent]
}

type Continent {
  code: String
  name: String
  countries: [Country]
}

type Country {
  code: String
  name: String
  nativeName: String
  phone: String
  continentCode: String
  currencyAlphabeticCode: String
  languages: [CountryLanguage]
  continent: Continent
  currency: Currency
  cities: [City]
}

type City {
  id: Int!
  name: String
  latitude: Decimal!
  longitude: Decimal!
  countryCode: String
  country: Country
  isCapital: Boolean!
}

# Auto-mapped CountryLanguage and Currency types are omited
```

### Explicit Mapping

[Auto mapping](#auto-mapping) is a handy way to build GraphQL schema quickly (e.g. for prototyping) but this technique has a few drawbacks: since it works recursively, it is possible to get access to the data which is not supposed to be accessible (e.g. by security reasons). Epam.GraphQL provides two ways to solve this issue:
* Inline mapping
* Projection mapping

#### Inline Mapping

Let's say you do not want to expose field `countries` for `continents` in [the example above](#query-example). The first way to achieve this goal is to pass the second argument to a `FromIQueryable` call, which configures an object GraphQL type:

```csharp
Field("continentsWithoutCountriesField")
	.FromIQueryable(
	    context => context.DbContext.Continents,
	    builder =>
	    {
	        builder.Field(continent => continent.Code);
	        builder.Field(continent => continent.Name);
	    });
```

This is translated to the following GraphQL schema:

```graphql
type GraphQLQuery {
  continentsWithoutCountries: [GraphQLQueryContinentsWithoutCountriesField]
}

type GraphQLQueryContinentsWithoutCountriesField {
  code: String
  name: String
}
```

Epam.GraphQL generates a unique name for an underlying GraphQL type, depending on a name of query type and field name. It is possible to change the name of this type:

```csharp
builder.Name = "ContinentWithoutCountries";
```

#### Projection Mapping

[Inline mapping](#inline-mapping) is not convenient when you want to reuse CLR to GraphQL type mapping.
Assume you want to expose two fields, `allCities` and `capitals` from GraphQL query, with the same model type [`City`](00-samples-ef-schema.md#city) and to restrict fields of the model by two fields, `id` and `name`:

```csharp
public class GraphQLQuery : Query<GraphQLExecutionContext>
{
	protected override void OnConfigure()
	{
		Field("allCities")
			.FromIQueryable(
				context => context.DbContext.Cities,
				builder =>
				{
					builder.Field(city => city.Id);
					builder.Field(city => city.Name);
				});

		Field("capitals")
			.FromIQueryable(
				context => context.DbContext.Cities.Where(city => city.IsCapital),
				builder =>
				{
					builder.Field(city => city.Id);
					builder.Field(city => city.Name);
				});
	}
}
```

Obviously, this implementation of `OnConfigure` contains code duplication and the better solutions is to define mapping as a projection and use this projection for configuring GraphQL type mapping:

```csharp
public class CityProjection : Projection<City, GraphQLExecutionContext>
{
	protected override void OnConfigure()
	{
		Field(city => city.Id);
		Field(city => city.Name);
	}
}

public class GraphQLQuery : Query<GraphQLExecutionContext>
{
	protected override void OnConfigure()
	{
		Field("allCities")
			.FromIQueryable(
				context => context.DbContext.Cities,
				builder => builder.ConfigureFrom<CityProjection>());

		Field("capitals")
			.FromIQueryable(
				context => context.DbContext.Cities.Where(city => city.IsCapital),
				builder => builder.ConfigureFrom<CityProjection>());
	}
}


```

A projection of a model is a class, inherited from an abstract class `Epam.GraphQL.Loaders.Projection<TEntity, TExecutionContext>`, which has two type parameters - `TEntity` (model) and [`TExecutionContext`](#execution-context). In order to implement projection for a particular model, `OnConfigure` method has to be overridden; the body of this method has to define fields, which will be available via corresponding GraphQL type.
Implemented projection can be used for configuring GraphQL type mapping:
```csharp
builder => builder.ConfigureFrom<CityProjection>()
```

## Loader 

Loader is a further development of [projection](#projection-mapping) idea. As a projection, it contains definition how to map a model to a GraphQL type, but also it implements how data should be retrieved from [execution context](#execution-context). Loaders are supposed to be building blocks for Epam.GraphQL API. Similarly to a projection, a loader is a class, inherited from `Epam.GraphQL.Loaders.Loader<TEntity, TExecutionContext>`, which has two type parameters - `TEntity` (model) and `TExecutionContext`. Implementing a loader for a particular entity, two abstract methods have to be overridden at least:
* `void OnConfigure()` - how entity will be mapped to a GraphQL type
* `IQueryable<TEntity> GetBaseQuery(TExecutionContext context)` - how entities will be retrieved via `TExecutionContext`

Let's reimplement [this example](#inline-mapping) using loader:
```csharp
public class ContinentLoader : Loader<Continent, GraphQLExecutionContext>
{
	protected override void OnConfigure()
	{
		Field(continent => continent.Code);
		Field(continent => continent.Name);
	}

	protected override IQueryable<Continent> GetBaseQuery(GraphQLExecutionContext context)
	{
		return context.DbContext.Continents;
	}
}

public class GraphQLQuery : Query<GraphQLExecutionContext>
{
	protected override void OnConfigure()
	{
		Field("continentsWithoutCountriesField")
			.FromLoader<ContinentLoader, Continent>();
}
```

## Mutation

TBD

## Schema Executer

TBD