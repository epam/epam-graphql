using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class Language
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string NativeName { get; set; }

        public bool RightToLeft { get; set; }

        public ICollection<CountryLanguage> Countries { get; set; }
    }
}
