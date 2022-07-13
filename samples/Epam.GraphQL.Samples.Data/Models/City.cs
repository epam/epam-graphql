using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string CountryCode { get; set; }

        public Country Country { get; set; }

        public bool IsCapital { get; set; }
    }
}
