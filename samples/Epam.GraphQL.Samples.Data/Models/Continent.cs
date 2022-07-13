using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class Continent
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<Country> Countries { get; set; }
    }
}
