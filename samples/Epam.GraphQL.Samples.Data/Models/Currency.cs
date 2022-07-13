using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class Currency
    {
        public string Name { get; set; }

        public string AlphabeticCode { get; set; }

        public string NumericCode { get; set; }

        public int? MinorUnit { get; set; }

        public ICollection<Country> Countries { get; set; }
    }
}
