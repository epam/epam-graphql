using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class CountryLanguage
    {
        public string CountryCode { get; set; }

        public string LanguageCode { get; set; }

        public Country Country { get; set; }

        public Language Language { get; set; }
    }
}
