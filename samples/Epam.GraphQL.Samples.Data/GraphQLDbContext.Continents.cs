using System;
using System.Collections.Generic;
using Epam.GraphQL.Samples.Data.Models;

namespace Epam.GraphQL.Samples.Data
{
    public partial class GraphQLDbContext
    {
        private static IEnumerable<Continent> GetContinents()
        {
            yield return new Continent { Code = "AF", Name = "Africa" };
            yield return new Continent { Code = "AN", Name = "Antarctica" };
            yield return new Continent { Code = "AS", Name = "Asia" };
            yield return new Continent { Code = "EU", Name = "Europe" };
            yield return new Continent { Code = "NA", Name = "North America" };
            yield return new Continent { Code = "OC", Name = "Oceania" };
            yield return new Continent { Code = "SA", Name = "South America" };
        }
    }
}
