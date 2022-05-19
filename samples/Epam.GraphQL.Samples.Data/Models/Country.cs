using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class Country
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string NativeName { get; set; }

        public string Phone { get; set; }

        public string ContinentCode { get; set; }

        public string CurrencyAlphabeticCode { get; set; }

        public ICollection<CountryLanguage> Languages { get; set; }

        public Continent Continent { get; set; }

        public Currency Currency { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
