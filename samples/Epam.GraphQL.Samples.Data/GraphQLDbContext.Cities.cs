using System;
using System.Collections.Generic;
using Epam.GraphQL.Samples.Data.Models;

namespace Epam.GraphQL.Samples.Data
{
    public partial class GraphQLDbContext
    {
        private static readonly HashSet<int> _capitalIds = new HashSet<int>
        {
            3041563,
            292968,
            1138958,
            3576022,
            3573374,
            3183875,
            616052,
            2240449,
            3435910,
            5881576,
            2761369,
            2172517,
            3577154,
            3041732,
            587084,
            3191281,
            3374036,
            1185241,
            2800866,
            2357048,
            727011,
            290340,
            425378,
            2392087,
            3579132,
            5969782,
            1820906,
            3903987,
            3513563,
            3469058,
            3571824,
            1252416,
            933773,
            625144,
            3582672,
            6094817,
            7304591,
            2314302,
            2389853,
            2260535,
            2661552,
            2279755,
            4035715,
            3871336,
            2220957,
            1816670,
            3688689,
            3621841,
            3553478,
            3374333,
            3513090,
            2078127,
            146268,
            3067696,
            2950159,
            223817,
            2618425,
            3575635,
            3492908,
            2507480,
            3652462,
            588409,
            360630,
            2462881,
            343300,
            3117735,
            344979,
            658225,
            2198148,
            3426691,
            2081986,
            2611396,
            2988507,
            2399697,
            2643743,
            3579925,
            611717,
            3382160,
            3042287,
            2306104,
            2411585,
            3421319,
            2413876,
            2422465,
            3579732,
            2309527,
            264371,
            3598132,
            4044012,
            2374775,
            3378644,
            1819729,
            3600949,
            3186886,
            3718426,
            3054643,
            1642911,
            2964574,
            281184,
            3042237,
            1261481,
            98182,
            112931,
            3413829,
            3169070,
            3042091,
            3489854,
            250441,
            1850147,
            184745,
            1528675,
            1821306,
            2110257,
            921772,
            3575551,
            1871859,
            1835848,
            285787,
            3580661,
            1526273,
            1651944,
            276781,
            3576812,
            3042030,
            1248991,
            2274895,
            932505,
            593116,
            2960316,
            456172,
            2210247,
            2538475,
            2993458,
            618426,
            3193044,
            3578851,
            1070940,
            2113779,
            785842,
            2460596,
            6611854,
            2028462,
            7828758,
            3570675,
            2377450,
            3578069,
            2562305,
            934154,
            1282027,
            927967,
            3530597,
            1735161,
            1040652,
            3352136,
            2139521,
            2440485,
            3489854,
            2352778,
            3617763,
            2759794,
            3143244,
            1283240,
            7626461,
            4036284,
            2179537,
            287286,
            3703443,
            3936456,
            4033936,
            2088122,
            1701668,
            1176615,
            756135,
            3424934,
            4030723,
            4568127,
            282239,
            2267057,
            3439389,
            290030,
            935264,
            683506,
            792680,
            524901,
            202061,
            108410,
            2108502,
            241131,
            379252,
            2673730,
            1880252,
            3370903,
            3196359,
            2729907,
            3060972,
            2409306,
            3168070,
            2253354,
            53654,
            3383330,
            373303,
            2410763,
            3583361,
            3513392,
            170654,
            935048,
            3576994,
            2427123,
            1546102,
            2365267,
            1609350,
            1221874,
            1645457,
            162183,
            2464470,
            4032402,
            323786,
            3573890,
            2110394,
            1668341,
            160196,
            703448,
            232422,
            4140963,
            3441575,
            1512569,
            6691831,
            3577887,
            3646738,
            3577430,
            4795467,
            1581130,
            2135171,
            4034821,
            4035413,
            786714,
            71137,
            921815,
            964137,
            909137,
            890299,
        };

        private static IEnumerable<City> GetCities()
        {
            foreach (var city in GetCitiesCore())
            {
                city.IsCapital = _capitalIds.Contains(city.Id);
                yield return city;
            }

            static IEnumerable<City> GetCitiesCore()
            {
                yield return new City { Id = 3041563, Name = "Andorra la Vella", Latitude = 42.50779m, Longitude = 1.52109m, CountryCode = "AD" };
                yield return new City { Id = 292223, Name = "Dubai", Latitude = 25.0657m, Longitude = 55.17128m, CountryCode = "AE" };
                yield return new City { Id = 292672, Name = "Sharjah", Latitude = 25.33737m, Longitude = 55.41206m, CountryCode = "AE" };
                yield return new City { Id = 292968, Name = "Abu Dhabi", Latitude = 24.46667m, Longitude = 54.36667m, CountryCode = "AE" };
                yield return new City { Id = 1138958, Name = "Kabul", Latitude = 34.52813m, Longitude = 69.17233m, CountryCode = "AF" };
                yield return new City { Id = 3576022, Name = "Saint John's", Latitude = 17.11717m, Longitude = -61.84573m, CountryCode = "AG" };
                yield return new City { Id = 3573374, Name = "The Valley", Latitude = 18.21704m, Longitude = -63.05783m, CountryCode = "AI" };
                yield return new City { Id = 3183875, Name = "Tirana", Latitude = 41.3275m, Longitude = 19.81889m, CountryCode = "AL" };
                yield return new City { Id = 616052, Name = "Yerevan", Latitude = 40.18111m, Longitude = 44.51361m, CountryCode = "AM" };
                yield return new City { Id = 2240449, Name = "Luanda", Latitude = -8.83682m, Longitude = 13.23432m, CountryCode = "AO" };
                yield return new City { Id = 3429652, Name = "Quilmes", Latitude = -34.72904m, Longitude = -58.26374m, CountryCode = "AR" };
                yield return new City { Id = 3430863, Name = "Mar del Plata", Latitude = -38.00228m, Longitude = -57.55754m, CountryCode = "AR" };
                yield return new City { Id = 3432043, Name = "La Plata", Latitude = -34.92145m, Longitude = -57.95453m, CountryCode = "AR" };
                yield return new City { Id = 3435910, Name = "Buenos Aires", Latitude = -34.61315m, Longitude = -58.37723m, CountryCode = "AR" };
                yield return new City { Id = 3832934, Name = "Victoria", Latitude = -32.61841m, Longitude = -60.15478m, CountryCode = "AR" };
                yield return new City { Id = 3836873, Name = "San Miguel de Tucumán", Latitude = -26.82414m, Longitude = -65.2226m, CountryCode = "AR" };
                yield return new City { Id = 3837213, Name = "San Juan", Latitude = -31.5375m, Longitude = -68.53639m, CountryCode = "AR" };
                yield return new City { Id = 3838233, Name = "Salta", Latitude = -24.7859m, Longitude = -65.41166m, CountryCode = "AR" };
                yield return new City { Id = 3838583, Name = "Rosario", Latitude = -32.94682m, Longitude = -60.63932m, CountryCode = "AR" };
                yield return new City { Id = 3844421, Name = "Mendoza", Latitude = -32.89084m, Longitude = -68.82717m, CountryCode = "AR" };
                yield return new City { Id = 3860259, Name = "Córdoba", Latitude = -31.4135m, Longitude = -64.18105m, CountryCode = "AR" };
                yield return new City { Id = 5881576, Name = "Pago Pago", Latitude = -14.27806m, Longitude = -170.7025m, CountryCode = "AS" };
                yield return new City { Id = 2761369, Name = "Vienna", Latitude = 48.20849m, Longitude = 16.37208m, CountryCode = "AT" };
                yield return new City { Id = 2063523, Name = "Perth", Latitude = -31.95224m, Longitude = 115.8614m, CountryCode = "AU" };
                yield return new City { Id = 2078025, Name = "Adelaide", Latitude = -34.92866m, Longitude = 138.59863m, CountryCode = "AU" };
                yield return new City { Id = 2147714, Name = "Sydney", Latitude = -33.86785m, Longitude = 151.20732m, CountryCode = "AU" };
                yield return new City { Id = 2158177, Name = "Melbourne", Latitude = -37.814m, Longitude = 144.96332m, CountryCode = "AU" };
                yield return new City { Id = 2165087, Name = "Gold Coast", Latitude = -28.00029m, Longitude = 153.43088m, CountryCode = "AU" };
                yield return new City { Id = 2172517, Name = "Canberra", Latitude = -35.28346m, Longitude = 149.12807m, CountryCode = "AU" };
                yield return new City { Id = 2174003, Name = "Brisbane", Latitude = -27.46794m, Longitude = 153.02809m, CountryCode = "AU" };
                yield return new City { Id = 3577154, Name = "Oranjestad", Latitude = 12.52398m, Longitude = -70.02703m, CountryCode = "AW" };
                yield return new City { Id = 3041732, Name = "Mariehamn", Latitude = 60.09726m, Longitude = 19.93481m, CountryCode = "AX" };
                yield return new City { Id = 587084, Name = "Baku", Latitude = 40.37767m, Longitude = 49.89201m, CountryCode = "AZ" };
                yield return new City { Id = 3191281, Name = "Sarajevo", Latitude = 43.84864m, Longitude = 18.35644m, CountryCode = "BA" };
                yield return new City { Id = 3374036, Name = "Bridgetown", Latitude = 13.10732m, Longitude = -59.62021m, CountryCode = "BB" };
                yield return new City { Id = 1185128, Name = "Rājshāhi", Latitude = 24.374m, Longitude = 88.60114m, CountryCode = "BD" };
                yield return new City { Id = 1185241, Name = "Dhaka", Latitude = 23.7104m, Longitude = 90.40744m, CountryCode = "BD" };
                yield return new City { Id = 1205733, Name = "Chittagong", Latitude = 22.3384m, Longitude = 91.83168m, CountryCode = "BD" };
                yield return new City { Id = 1336135, Name = "Khulna", Latitude = 22.80979m, Longitude = 89.56439m, CountryCode = "BD" };
                yield return new City { Id = 2800866, Name = "Brussels", Latitude = 50.85045m, Longitude = 4.34878m, CountryCode = "BE" };
                yield return new City { Id = 2357048, Name = "Ouagadougou", Latitude = 12.36566m, Longitude = -1.53388m, CountryCode = "BF" };
                yield return new City { Id = 727011, Name = "Sofia", Latitude = 42.69751m, Longitude = 23.32415m, CountryCode = "BG" };
                yield return new City { Id = 290340, Name = "Manama", Latitude = 26.22787m, Longitude = 50.58565m, CountryCode = "BH" };
                yield return new City { Id = 425378, Name = "Bujumbura", Latitude = -3.3822m, Longitude = 29.3644m, CountryCode = "BI" };
                yield return new City { Id = 2392087, Name = "Porto-Novo", Latitude = 6.49646m, Longitude = 2.60359m, CountryCode = "BJ" };
                yield return new City { Id = 2394819, Name = "Cotonou", Latitude = 6.36536m, Longitude = 2.41833m, CountryCode = "BJ" };
                yield return new City { Id = 3579132, Name = "Gustavia", Latitude = 17.89618m, Longitude = -62.84978m, CountryCode = "BL" };
                yield return new City { Id = 1820906, Name = "Bandar Seri Begawan", Latitude = 4.94029m, Longitude = 114.94806m, CountryCode = "BN" };
                yield return new City { Id = 3903987, Name = "Sucre", Latitude = -19.03332m, Longitude = -65.26274m, CountryCode = "BO" };
                yield return new City { Id = 3904906, Name = "Santa Cruz de la Sierra", Latitude = -17.78629m, Longitude = -63.18117m, CountryCode = "BO" };
                yield return new City { Id = 3911925, Name = "La Paz", Latitude = -16.5m, Longitude = -68.15m, CountryCode = "BO" };
                yield return new City { Id = 3919968, Name = "Cochabamba", Latitude = -17.3895m, Longitude = -66.1568m, CountryCode = "BO" };
                yield return new City { Id = 3513563, Name = "Kralendijk", Latitude = 12.15m, Longitude = -68.26667m, CountryCode = "BQ" };
                yield return new City { Id = 3384987, Name = "Victoria", Latitude = -8.11806m, Longitude = -35.29139m, CountryCode = "BR" };
                yield return new City { Id = 3386496, Name = "Teresina", Latitude = -5.08917m, Longitude = -42.80194m, CountryCode = "BR" };
                yield return new City { Id = 3388368, Name = "São Luís", Latitude = -2.52972m, Longitude = -44.30278m, CountryCode = "BR" };
                yield return new City { Id = 3390760, Name = "Recife", Latitude = -8.05389m, Longitude = -34.88111m, CountryCode = "BR" };
                yield return new City { Id = 3394023, Name = "Natal", Latitude = -5.795m, Longitude = -35.20944m, CountryCode = "BR" };
                yield return new City { Id = 3395981, Name = "Maceió", Latitude = -9.66583m, Longitude = -35.73528m, CountryCode = "BR" };
                yield return new City { Id = 3397277, Name = "João Pessoa", Latitude = -7.115m, Longitude = -34.86306m, CountryCode = "BR" };
                yield return new City { Id = 3397838, Name = "Jaboatão", Latitude = -8.18028m, Longitude = -35.00139m, CountryCode = "BR" };
                yield return new City { Id = 3399415, Name = "Fortaleza", Latitude = -3.71722m, Longitude = -38.54306m, CountryCode = "BR" };
                yield return new City { Id = 3405870, Name = "Belém", Latitude = -1.45583m, Longitude = -48.50444m, CountryCode = "BR" };
                yield return new City { Id = 3444924, Name = "Victoria", Latitude = -20.31944m, Longitude = -40.33778m, CountryCode = "BR" };
                yield return new City { Id = 3445831, Name = "Uberlândia", Latitude = -18.91861m, Longitude = -48.27722m, CountryCode = "BR" };
                yield return new City { Id = 3447399, Name = "Sorocaba", Latitude = -23.50167m, Longitude = -47.45806m, CountryCode = "BR" };
                yield return new City { Id = 3448439, Name = "São Paulo", Latitude = -23.5475m, Longitude = -46.63611m, CountryCode = "BR" };
                yield return new City { Id = 3448636, Name = "São José dos Campos", Latitude = -23.17944m, Longitude = -45.88694m, CountryCode = "BR" };
                yield return new City { Id = 3449344, Name = "São Bernardo do Campo", Latitude = -23.69389m, Longitude = -46.565m, CountryCode = "BR" };
                yield return new City { Id = 3449701, Name = "Santo André", Latitude = -23.66389m, Longitude = -46.53833m, CountryCode = "BR" };
                yield return new City { Id = 3450554, Name = "Salvador", Latitude = -12.97111m, Longitude = -38.51083m, CountryCode = "BR" };
                yield return new City { Id = 3451190, Name = "Rio de Janeiro", Latitude = -22.90278m, Longitude = -43.2075m, CountryCode = "BR" };
                yield return new City { Id = 3451328, Name = "Ribeirão Preto", Latitude = -21.1775m, Longitude = -47.81028m, CountryCode = "BR" };
                yield return new City { Id = 3452925, Name = "Porto Alegre", Latitude = -30.03306m, Longitude = -51.23m, CountryCode = "BR" };
                yield return new City { Id = 3455775, Name = "Osasco", Latitude = -23.5325m, Longitude = -46.79167m, CountryCode = "BR" };
                yield return new City { Id = 3456160, Name = "Nova Iguaçu", Latitude = -22.75917m, Longitude = -43.45111m, CountryCode = "BR" };
                yield return new City { Id = 3461786, Name = "Guarulhos", Latitude = -23.46278m, Longitude = -46.53333m, CountryCode = "BR" };
                yield return new City { Id = 3462377, Name = "Goiânia", Latitude = -16.67861m, Longitude = -49.25389m, CountryCode = "BR" };
                yield return new City { Id = 3464374, Name = "Duque de Caxias", Latitude = -22.78556m, Longitude = -43.31167m, CountryCode = "BR" };
                yield return new City { Id = 3464975, Name = "Curitiba", Latitude = -25.42778m, Longitude = -49.27306m, CountryCode = "BR" };
                yield return new City { Id = 3465038, Name = "Cuiabá", Latitude = -15.59611m, Longitude = -56.09667m, CountryCode = "BR" };
                yield return new City { Id = 3465624, Name = "Contagem", Latitude = -19.93167m, Longitude = -44.05361m, CountryCode = "BR" };
                yield return new City { Id = 3467747, Name = "Campo Grande", Latitude = -20.44278m, Longitude = -54.64639m, CountryCode = "BR" };
                yield return new City { Id = 3467865, Name = "Campinas", Latitude = -22.90556m, Longitude = -47.06083m, CountryCode = "BR" };
                yield return new City { Id = 3469058, Name = "Brasília", Latitude = -15.77972m, Longitude = -47.92972m, CountryCode = "BR" };
                yield return new City { Id = 3470127, Name = "Belo Horizonte", Latitude = -19.92083m, Longitude = -43.93778m, CountryCode = "BR" };
                yield return new City { Id = 3663517, Name = "Manaus", Latitude = -3.10194m, Longitude = -60.025m, CountryCode = "BR" };
                yield return new City { Id = 6316406, Name = "Aparecida de Goiânia", Latitude = -16.82333m, Longitude = -49.24389m, CountryCode = "BR" };
                yield return new City { Id = 6317344, Name = "Jaboatão dos Guararapes", Latitude = -8.11278m, Longitude = -35.01472m, CountryCode = "BR" };
                yield return new City { Id = 3571824, Name = "Nassau", Latitude = 25.05823m, Longitude = -77.34306m, CountryCode = "BS" };
                yield return new City { Id = 1252416, Name = "Thimphu", Latitude = 27.46609m, Longitude = 89.64191m, CountryCode = "BT" };
                yield return new City { Id = 933773, Name = "Gaborone", Latitude = -24.65451m, Longitude = 25.90859m, CountryCode = "BW" };
                yield return new City { Id = 625144, Name = "Minsk", Latitude = 53.9m, Longitude = 27.56667m, CountryCode = "BY" };
                yield return new City { Id = 3582672, Name = "Belmopan", Latitude = 17.25m, Longitude = -88.76667m, CountryCode = "BZ" };
                yield return new City { Id = 5913490, Name = "Calgary", Latitude = 51.05011m, Longitude = -114.08529m, CountryCode = "CA" };
                yield return new City { Id = 5946768, Name = "Edmonton", Latitude = 53.55014m, Longitude = -113.46871m, CountryCode = "CA" };
                yield return new City { Id = 5969782, Name = "Hamilton", Latitude = 43.25011m, Longitude = -79.84963m, CountryCode = "CA" };
                yield return new City { Id = 6075357, Name = "Mississauga", Latitude = 43.5789m, Longitude = -79.6583m, CountryCode = "CA" };
                yield return new City { Id = 6077243, Name = "Montréal", Latitude = 45.50884m, Longitude = -73.58781m, CountryCode = "CA" };
                yield return new City { Id = 6091104, Name = "North York", Latitude = 43.76681m, Longitude = -79.4163m, CountryCode = "CA" };
                yield return new City { Id = 6094817, Name = "Ottawa", Latitude = 45.41117m, Longitude = -75.69812m, CountryCode = "CA" };
                yield return new City { Id = 6167865, Name = "Toronto", Latitude = 43.70011m, Longitude = -79.4163m, CountryCode = "CA" };
                yield return new City { Id = 6173331, Name = "Vancouver", Latitude = 49.24966m, Longitude = -123.11934m, CountryCode = "CA" };
                yield return new City { Id = 6174041, Name = "Victoria", Latitude = 48.43294m, Longitude = -123.3693m, CountryCode = "CA" };
                yield return new City { Id = 6183235, Name = "Winnipeg", Latitude = 49.8844m, Longitude = -97.14704m, CountryCode = "CA" };
                yield return new City { Id = 6324733, Name = "Saint John's", Latitude = 47.56494m, Longitude = -52.70931m, CountryCode = "CA" };
                yield return new City { Id = 6325494, Name = "Québec", Latitude = 46.81228m, Longitude = -71.21454m, CountryCode = "CA" };
                yield return new City { Id = 6948711, Name = "Scarborough", Latitude = 43.77223m, Longitude = -79.25666m, CountryCode = "CA" };
                yield return new City { Id = 7304591, Name = "West Island", Latitude = -12.15681m, Longitude = 96.82251m, CountryCode = "CC" };
                yield return new City { Id = 209228, Name = "Mbuji-Mayi", Latitude = -6.13603m, Longitude = 23.58979m, CountryCode = "CD" };
                yield return new City { Id = 212730, Name = "Kisangani", Latitude = 0.51528m, Longitude = 25.19099m, CountryCode = "CD" };
                yield return new City { Id = 922704, Name = "Lubumbashi", Latitude = -11.66089m, Longitude = 27.47938m, CountryCode = "CD" };
                yield return new City { Id = 2314302, Name = "Kinshasa", Latitude = -4.32758m, Longitude = 15.31357m, CountryCode = "CD" };
                yield return new City { Id = 2389853, Name = "Bangui", Latitude = 4.36122m, Longitude = 18.55496m, CountryCode = "CF" };
                yield return new City { Id = 2255414, Name = "Pointe-Noire", Latitude = -4.77609m, Longitude = 11.86352m, CountryCode = "CG" };
                yield return new City { Id = 2260535, Name = "Brazzaville", Latitude = -4.26613m, Longitude = 15.28318m, CountryCode = "CG" };
                yield return new City { Id = 2661552, Name = "Bern", Latitude = 46.94809m, Longitude = 7.44744m, CountryCode = "CH" };
                yield return new City { Id = 2279755, Name = "Yamoussoukro", Latitude = 6.82055m, Longitude = -5.27674m, CountryCode = "CI" };
                yield return new City { Id = 2290956, Name = "Bouaké", Latitude = 7.69385m, Longitude = -5.03031m, CountryCode = "CI" };
                yield return new City { Id = 2293521, Name = "Abobo", Latitude = 5.41613m, Longitude = -4.0159m, CountryCode = "CI" };
                yield return new City { Id = 2293538, Name = "Abidjan", Latitude = 5.30966m, Longitude = -4.01266m, CountryCode = "CI" };
                yield return new City { Id = 4035715, Name = "Avarua", Latitude = -21.20778m, Longitude = -159.775m, CountryCode = "CK" };
                yield return new City { Id = 3868326, Name = "Victoria", Latitude = -38.23291m, Longitude = -72.33292m, CountryCode = "CL" };
                yield return new City { Id = 3871336, Name = "Santiago", Latitude = -33.45694m, Longitude = -70.64827m, CountryCode = "CL" };
                yield return new City { Id = 3875024, Name = "Puente Alto", Latitude = -33.61169m, Longitude = -70.57577m, CountryCode = "CL" };
                yield return new City { Id = 2220957, Name = "Yaoundé", Latitude = 3.86667m, Longitude = 11.51667m, CountryCode = "CM" };
                yield return new City { Id = 2229411, Name = "Victoria", Latitude = 4.02356m, Longitude = 9.20607m, CountryCode = "CM" };
                yield return new City { Id = 2232593, Name = "Douala", Latitude = 4.04827m, Longitude = 9.70428m, CountryCode = "CM" };
                yield return new City { Id = 1529102, Name = "Ürümqi", Latitude = 43.80096m, Longitude = 87.60046m, CountryCode = "CN" };
                yield return new City { Id = 1529195, Name = "Shihezi", Latitude = 44.3023m, Longitude = 86.03694m, CountryCode = "CN" };
                yield return new City { Id = 1783745, Name = "Zigong", Latitude = 29.34162m, Longitude = 104.77689m, CountryCode = "CN" };
                yield return new City { Id = 1783763, Name = "Zhuzhou", Latitude = 27.83333m, Longitude = 113.15m, CountryCode = "CN" };
                yield return new City { Id = 1783873, Name = "Zhumadian", Latitude = 32.97944m, Longitude = 114.02944m, CountryCode = "CN" };
                yield return new City { Id = 1784642, Name = "Zhenjiang", Latitude = 32.21086m, Longitude = 119.45508m, CountryCode = "CN" };
                yield return new City { Id = 1784658, Name = "Zhengzhou", Latitude = 34.75778m, Longitude = 113.64861m, CountryCode = "CN" };
                yield return new City { Id = 1784990, Name = "Zhanjiang", Latitude = 21.28145m, Longitude = 110.34271m, CountryCode = "CN" };
                yield return new City { Id = 1785018, Name = "Zhangzhou", Latitude = 24.51333m, Longitude = 117.65556m, CountryCode = "CN" };
                yield return new City { Id = 1785286, Name = "Zibo", Latitude = 36.79056m, Longitude = 118.06333m, CountryCode = "CN" };
                yield return new City { Id = 1785294, Name = "Anyang", Latitude = 36.096m, Longitude = 114.38278m, CountryCode = "CN" };
                yield return new City { Id = 1785725, Name = "Yunfu", Latitude = 22.92833m, Longitude = 112.03954m, CountryCode = "CN" };
                yield return new City { Id = 1787093, Name = "Yantai", Latitude = 37.47649m, Longitude = 121.44081m, CountryCode = "CN" };
                yield return new City { Id = 1787227, Name = "Yangzhou", Latitude = 32.39722m, Longitude = 119.43583m, CountryCode = "CN" };
                yield return new City { Id = 1787746, Name = "Yancheng", Latitude = 33.3575m, Longitude = 120.1573m, CountryCode = "CN" };
                yield return new City { Id = 1787824, Name = "Tongshan", Latitude = 34.18045m, Longitude = 117.15707m, CountryCode = "CN" };
                yield return new City { Id = 1788534, Name = "Xinyang", Latitude = 32.12278m, Longitude = 114.06556m, CountryCode = "CN" };
                yield return new City { Id = 1788572, Name = "Nangandao", Latitude = 35.19033m, Longitude = 113.80151m, CountryCode = "CN" };
                yield return new City { Id = 1788852, Name = "Xining", Latitude = 36.62554m, Longitude = 101.75739m, CountryCode = "CN" };
                yield return new City { Id = 1788927, Name = "Xingtai", Latitude = 37.06306m, Longitude = 114.49417m, CountryCode = "CN" };
                yield return new City { Id = 1790353, Name = "Xianyang", Latitude = 34.33778m, Longitude = 108.70261m, CountryCode = "CN" };
                yield return new City { Id = 1790437, Name = "Zhuhai", Latitude = 22.27694m, Longitude = 113.56778m, CountryCode = "CN" };
                yield return new City { Id = 1790492, Name = "Xiangtan", Latitude = 27.85m, Longitude = 112.9m, CountryCode = "CN" };
                yield return new City { Id = 1790630, Name = "Xi’an", Latitude = 34.25833m, Longitude = 108.92861m, CountryCode = "CN" };
                yield return new City { Id = 1790645, Name = "Xiamen", Latitude = 24.47979m, Longitude = 118.08187m, CountryCode = "CN" };
                yield return new City { Id = 1790923, Name = "Wuxi", Latitude = 31.56887m, Longitude = 120.28857m, CountryCode = "CN" };
                yield return new City { Id = 1791121, Name = "Changde", Latitude = 29.04638m, Longitude = 111.6783m, CountryCode = "CN" };
                yield return new City { Id = 1791236, Name = "Wuhu", Latitude = 31.33728m, Longitude = 118.37351m, CountryCode = "CN" };
                yield return new City { Id = 1791247, Name = "Wuhan", Latitude = 30.58333m, Longitude = 114.26667m, CountryCode = "CN" };
                yield return new City { Id = 1791388, Name = "Wenzhou", Latitude = 27.99942m, Longitude = 120.66682m, CountryCode = "CN" };
                yield return new City { Id = 1792892, Name = "Tianshui", Latitude = 34.57952m, Longitude = 105.74238m, CountryCode = "CN" };
                yield return new City { Id = 1792947, Name = "Tianjin", Latitude = 39.14222m, Longitude = 117.17667m, CountryCode = "CN" };
                yield return new City { Id = 1793346, Name = "Tangshan", Latitude = 39.63333m, Longitude = 118.18333m, CountryCode = "CN" };
                yield return new City { Id = 1793424, Name = "Tanggu", Latitude = 39.02111m, Longitude = 117.64694m, CountryCode = "CN" };
                yield return new City { Id = 1793505, Name = "Taizhou", Latitude = 32.49069m, Longitude = 119.90812m, CountryCode = "CN" };
                yield return new City { Id = 1793511, Name = "Taiyuan", Latitude = 37.86944m, Longitude = 112.56028m, CountryCode = "CN" };
                yield return new City { Id = 1793724, Name = "Tai’an", Latitude = 36.18528m, Longitude = 117.12m, CountryCode = "CN" };
                yield return new City { Id = 1794903, Name = "Shiyan", Latitude = 32.6475m, Longitude = 110.77806m, CountryCode = "CN" };
                yield return new City { Id = 1795270, Name = "Shijiazhuang", Latitude = 38.04139m, Longitude = 114.47861m, CountryCode = "CN" };
                yield return new City { Id = 1795565, Name = "Shenzhen", Latitude = 22.54554m, Longitude = 114.0683m, CountryCode = "CN" };
                yield return new City { Id = 1795874, Name = "Shaoguan", Latitude = 24.8m, Longitude = 113.58333m, CountryCode = "CN" };
                yield return new City { Id = 1795940, Name = "Shantou", Latitude = 23.36814m, Longitude = 116.71479m, CountryCode = "CN" };
                yield return new City { Id = 1796236, Name = "Shanghai", Latitude = 31.22222m, Longitude = 121.45806m, CountryCode = "CN" };
                yield return new City { Id = 1797121, Name = "Jieyang", Latitude = 23.5418m, Longitude = 116.36581m, CountryCode = "CN" };
                yield return new City { Id = 1797595, Name = "Qinhuangdao", Latitude = 39.93167m, Longitude = 119.58833m, CountryCode = "CN" };
                yield return new City { Id = 1797873, Name = "Huai'an", Latitude = 33.58861m, Longitude = 119.01917m, CountryCode = "CN" };
                yield return new City { Id = 1797929, Name = "Qingdao", Latitude = 36.06605m, Longitude = 120.36939m, CountryCode = "CN" };
                yield return new City { Id = 1798422, Name = "Puyang Chengguanzhen", Latitude = 35.70506m, Longitude = 115.01409m, CountryCode = "CN" };
                yield return new City { Id = 1798425, Name = "Puyang", Latitude = 29.45679m, Longitude = 119.88872m, CountryCode = "CN" };
                yield return new City { Id = 1798827, Name = "Pingdingshan", Latitude = 33.73847m, Longitude = 113.30119m, CountryCode = "CN" };
                yield return new City { Id = 1799397, Name = "Ningbo", Latitude = 29.87819m, Longitude = 121.54945m, CountryCode = "CN" };
                yield return new City { Id = 1799491, Name = "Neijiang", Latitude = 29.58354m, Longitude = 105.06216m, CountryCode = "CN" };
                yield return new City { Id = 1799722, Name = "Nantong", Latitude = 32.03028m, Longitude = 120.87472m, CountryCode = "CN" };
                yield return new City { Id = 1799869, Name = "Nanning", Latitude = 22.81667m, Longitude = 108.31667m, CountryCode = "CN" };
                yield return new City { Id = 1799962, Name = "Nanjing", Latitude = 32.06167m, Longitude = 118.77778m, CountryCode = "CN" };
                yield return new City { Id = 1800146, Name = "Nanchong", Latitude = 30.79508m, Longitude = 106.08473m, CountryCode = "CN" };
                yield return new City { Id = 1800163, Name = "Nanchang", Latitude = 28.68396m, Longitude = 115.85306m, CountryCode = "CN" };
                yield return new City { Id = 1801792, Name = "Luoyang", Latitude = 34.68361m, Longitude = 112.45361m, CountryCode = "CN" };
                yield return new City { Id = 1802204, Name = "Luancheng", Latitude = 37.87917m, Longitude = 114.65167m, CountryCode = "CN" };
                yield return new City { Id = 1802875, Name = "Guankou", Latitude = 28.15861m, Longitude = 113.62709m, CountryCode = "CN" };
                yield return new City { Id = 1804430, Name = "Lanzhou", Latitude = 36.05701m, Longitude = 103.83987m, CountryCode = "CN" };
                yield return new City { Id = 1804540, Name = "Langfang", Latitude = 39.50972m, Longitude = 116.69472m, CountryCode = "CN" };
                yield return new City { Id = 1804651, Name = "Kunming", Latitude = 25.03889m, Longitude = 102.71833m, CountryCode = "CN" };
                yield return new City { Id = 1804879, Name = "Kaifeng", Latitude = 34.7986m, Longitude = 114.30742m, CountryCode = "CN" };
                yield return new City { Id = 1805753, Name = "Jinan", Latitude = 36.66833m, Longitude = 116.99722m, CountryCode = "CN" };
                yield return new City { Id = 1805987, Name = "Jiaozuo", Latitude = 35.23972m, Longitude = 113.23306m, CountryCode = "CN" };
                yield return new City { Id = 1806299, Name = "Jiangmen", Latitude = 22.58333m, Longitude = 113.08333m, CountryCode = "CN" };
                yield return new City { Id = 1807234, Name = "Huangshi", Latitude = 30.24706m, Longitude = 115.04814m, CountryCode = "CN" };
                yield return new City { Id = 1807681, Name = "Huainan", Latitude = 32.62639m, Longitude = 116.99694m, CountryCode = "CN" };
                yield return new City { Id = 1807700, Name = "Huaibei", Latitude = 33.97444m, Longitude = 116.79167m, CountryCode = "CN" };
                yield return new City { Id = 1808370, Name = "Hengyang", Latitude = 26.88946m, Longitude = 112.61888m, CountryCode = "CN" };
                yield return new City { Id = 1808722, Name = "Hefei", Latitude = 31.86389m, Longitude = 117.28083m, CountryCode = "CN" };
                yield return new City { Id = 1808926, Name = "Hangzhou", Latitude = 30.29365m, Longitude = 120.16142m, CountryCode = "CN" };
                yield return new City { Id = 1808963, Name = "Handan", Latitude = 36.60056m, Longitude = 114.46778m, CountryCode = "CN" };
                yield return new City { Id = 1809078, Name = "Haikou", Latitude = 20.04583m, Longitude = 110.34167m, CountryCode = "CN" };
                yield return new City { Id = 1809412, Name = "Guli", Latitude = 28.88162m, Longitude = 120.03308m, CountryCode = "CN" };
                yield return new City { Id = 1809461, Name = "Guiyang", Latitude = 26.58333m, Longitude = 106.71667m, CountryCode = "CN" };
                yield return new City { Id = 1809498, Name = "Guilin", Latitude = 25.28194m, Longitude = 110.28639m, CountryCode = "CN" };
                yield return new City { Id = 1809858, Name = "Guangzhou", Latitude = 23.11667m, Longitude = 113.25m, CountryCode = "CN" };
                yield return new City { Id = 1810821, Name = "Fuzhou", Latitude = 26.06139m, Longitude = 119.30611m, CountryCode = "CN" };
                yield return new City { Id = 1811103, Name = "Foshan", Latitude = 23.02677m, Longitude = 113.13148m, CountryCode = "CN" };
                yield return new City { Id = 1812545, Name = "Dongguan", Latitude = 23.01797m, Longitude = 113.74866m, CountryCode = "CN" };
                yield return new City { Id = 1813253, Name = "Lijiang", Latitude = 26.86879m, Longitude = 100.22072m, CountryCode = "CN" };
                yield return new City { Id = 1814087, Name = "Dalian", Latitude = 38.91222m, Longitude = 121.60222m, CountryCode = "CN" };
                yield return new City { Id = 1814906, Name = "Chongqing", Latitude = 29.56278m, Longitude = 106.55278m, CountryCode = "CN" };
                yield return new City { Id = 1815286, Name = "Chengdu", Latitude = 30.66667m, Longitude = 104.06667m, CountryCode = "CN" };
                yield return new City { Id = 1815456, Name = "Changzhou", Latitude = 31.77359m, Longitude = 119.95401m, CountryCode = "CN" };
                yield return new City { Id = 1815463, Name = "Changzhi", Latitude = 35.20889m, Longitude = 111.73861m, CountryCode = "CN" };
                yield return new City { Id = 1815577, Name = "Changsha", Latitude = 28.19874m, Longitude = 112.97087m, CountryCode = "CN" };
                yield return new City { Id = 1816080, Name = "Cangzhou", Latitude = 38.31667m, Longitude = 116.86667m, CountryCode = "CN" };
                yield return new City { Id = 1816440, Name = "Bengbu", Latitude = 32.94083m, Longitude = 117.36083m, CountryCode = "CN" };
                yield return new City { Id = 1816670, Name = "Beijing", Latitude = 39.9075m, Longitude = 116.39723m, CountryCode = "CN" };
                yield return new City { Id = 1816971, Name = "Baoding", Latitude = 38.85111m, Longitude = 115.49028m, CountryCode = "CN" };
                yield return new City { Id = 1817720, Name = "Shangyu", Latitude = 30.01556m, Longitude = 120.87111m, CountryCode = "CN" };
                yield return new City { Id = 1886760, Name = "Suzhou", Latitude = 31.30408m, Longitude = 120.59538m, CountryCode = "CN" };
                yield return new City { Id = 1915223, Name = "Zhongshan", Latitude = 21.31992m, Longitude = 110.5723m, CountryCode = "CN" };
                yield return new City { Id = 1927639, Name = "Yueyang", Latitude = 29.37455m, Longitude = 113.09481m, CountryCode = "CN" };
                yield return new City { Id = 2033196, Name = "Zhangjiakou", Latitude = 40.81m, Longitude = 114.87944m, CountryCode = "CN" };
                yield return new City { Id = 2033370, Name = "Yingkou", Latitude = 40.66482m, Longitude = 122.22833m, CountryCode = "CN" };
                yield return new City { Id = 2034714, Name = "Siping", Latitude = 43.16143m, Longitude = 124.37785m, CountryCode = "CN" };
                yield return new City { Id = 2034786, Name = "Shuangyashan", Latitude = 46.63611m, Longitude = 131.15389m, CountryCode = "CN" };
                yield return new City { Id = 2034937, Name = "Shenyang", Latitude = 41.79222m, Longitude = 123.43278m, CountryCode = "CN" };
                yield return new City { Id = 2035265, Name = "Qiqihar", Latitude = 47.34088m, Longitude = 123.96045m, CountryCode = "CN" };
                yield return new City { Id = 2035513, Name = "Panshan", Latitude = 41.18806m, Longitude = 122.04944m, CountryCode = "CN" };
                yield return new City { Id = 2035715, Name = "Mudanjiang", Latitude = 44.58333m, Longitude = 129.6m, CountryCode = "CN" };
                yield return new City { Id = 2036113, Name = "Liaoyang", Latitude = 41.27194m, Longitude = 123.17306m, CountryCode = "CN" };
                yield return new City { Id = 2036427, Name = "Jinzhou", Latitude = 41.10778m, Longitude = 121.14167m, CountryCode = "CN" };
                yield return new City { Id = 2036502, Name = "Jilin", Latitude = 43.85083m, Longitude = 126.56028m, CountryCode = "CN" };
                yield return new City { Id = 2036581, Name = "Jiamusi", Latitude = 46.79927m, Longitude = 130.31633m, CountryCode = "CN" };
                yield return new City { Id = 2036892, Name = "Hohhot", Latitude = 40.81056m, Longitude = 111.65222m, CountryCode = "CN" };
                yield return new City { Id = 2036986, Name = "Hegang", Latitude = 47.35118m, Longitude = 130.30012m, CountryCode = "CN" };
                yield return new City { Id = 2037013, Name = "Harbin", Latitude = 45.75m, Longitude = 126.65m, CountryCode = "CN" };
                yield return new City { Id = 2037346, Name = "Fuxin", Latitude = 42.01556m, Longitude = 121.65889m, CountryCode = "CN" };
                yield return new City { Id = 2037355, Name = "Fushun", Latitude = 41.85583m, Longitude = 123.92333m, CountryCode = "CN" };
                yield return new City { Id = 2037799, Name = "Datong", Latitude = 40.09361m, Longitude = 113.29139m, CountryCode = "CN" };
                yield return new City { Id = 2037886, Name = "Dandong", Latitude = 40.12917m, Longitude = 124.39472m, CountryCode = "CN" };
                yield return new City { Id = 2038180, Name = "Changchun", Latitude = 43.88m, Longitude = 125.32278m, CountryCode = "CN" };
                yield return new City { Id = 2038300, Name = "Benxi", Latitude = 41.28861m, Longitude = 123.765m, CountryCode = "CN" };
                yield return new City { Id = 2038432, Name = "Baotou", Latitude = 40.65222m, Longitude = 109.82222m, CountryCode = "CN" };
                yield return new City { Id = 2038632, Name = "Anshan", Latitude = 41.12361m, Longitude = 122.99m, CountryCode = "CN" };
                yield return new City { Id = 7283386, Name = "Changshu City", Latitude = 31.64615m, Longitude = 120.74221m, CountryCode = "CN" };
                yield return new City { Id = 7602670, Name = "Zhu Cheng City", Latitude = 35.99502m, Longitude = 119.40259m, CountryCode = "CN" };
                yield return new City { Id = 8347664, Name = "Ordos", Latitude = 39.6086m, Longitude = 109.78157m, CountryCode = "CN" };
                yield return new City { Id = 3667728, Name = "Sucre", Latitude = 8.81136m, Longitude = -74.72084m, CountryCode = "CO" };
                yield return new City { Id = 3669454, Name = "San Juan", Latitude = 9.95157m, Longitude = -75.08198m, CountryCode = "CO" };
                yield return new City { Id = 3674962, Name = "Medellín", Latitude = 6.25184m, Longitude = -75.56359m, CountryCode = "CO" };
                yield return new City { Id = 3685533, Name = "Cúcuta", Latitude = 7.89391m, Longitude = -72.50782m, CountryCode = "CO" };
                yield return new City { Id = 3687238, Name = "Cartagena", Latitude = 10.39972m, Longitude = -75.51444m, CountryCode = "CO" };
                yield return new City { Id = 3687925, Name = "Cali", Latitude = 3.43722m, Longitude = -76.5225m, CountryCode = "CO" };
                yield return new City { Id = 3688465, Name = "Bucaramanga", Latitude = 7.12539m, Longitude = -73.1198m, CountryCode = "CO" };
                yield return new City { Id = 3688689, Name = "Bogotá", Latitude = 4.60971m, Longitude = -74.08175m, CountryCode = "CO" };
                yield return new City { Id = 3689147, Name = "Barranquilla", Latitude = 10.96854m, Longitude = -74.78132m, CountryCode = "CO" };
                yield return new City { Id = 3621819, Name = "San Juan", Latitude = 9.95974m, Longitude = -84.08165m, CountryCode = "CR" };
                yield return new City { Id = 3621841, Name = "San José", Latitude = 10.95173m, Longitude = -85.1361m, CountryCode = "CR" };
                yield return new City { Id = 3536729, Name = "Santiago de Cuba", Latitude = 20.02083m, Longitude = -75.82667m, CountryCode = "CU" };
                yield return new City { Id = 3550598, Name = "Victoria", Latitude = 20.96167m, Longitude = -76.95111m, CountryCode = "CU" };
                yield return new City { Id = 3553478, Name = "Havana", Latitude = 23.13302m, Longitude = -82.38304m, CountryCode = "CU" };
                yield return new City { Id = 3374333, Name = "Praia", Latitude = 14.93152m, Longitude = -23.51254m, CountryCode = "CV" };
                yield return new City { Id = 3513090, Name = "Willemstad", Latitude = 12.1084m, Longitude = -68.93354m, CountryCode = "CW" };
                yield return new City { Id = 2078127, Name = "Flying Fish Cove", Latitude = -10.42172m, Longitude = 105.67912m, CountryCode = "CX" };
                yield return new City { Id = 146268, Name = "Nicosia", Latitude = 35.17531m, Longitude = 33.3642m, CountryCode = "CY" };
                yield return new City { Id = 3067696, Name = "Prague", Latitude = 50.08804m, Longitude = 14.42076m, CountryCode = "CZ" };
                yield return new City { Id = 2825297, Name = "Stuttgart", Latitude = 48.78232m, Longitude = 9.17702m, CountryCode = "DE" };
                yield return new City { Id = 2867714, Name = "Munich", Latitude = 48.13743m, Longitude = 11.57549m, CountryCode = "DE" };
                yield return new City { Id = 2879139, Name = "Leipzig", Latitude = 51.33962m, Longitude = 12.37129m, CountryCode = "DE" };
                yield return new City { Id = 2886242, Name = "Köln", Latitude = 50.93333m, Longitude = 6.95m, CountryCode = "DE" };
                yield return new City { Id = 2910831, Name = "Hannover", Latitude = 52.37052m, Longitude = 9.73322m, CountryCode = "DE" };
                yield return new City { Id = 2911298, Name = "Hamburg", Latitude = 53.57532m, Longitude = 10.01534m, CountryCode = "DE" };
                yield return new City { Id = 2925533, Name = "Frankfurt am Main", Latitude = 50.11552m, Longitude = 8.68417m, CountryCode = "DE" };
                yield return new City { Id = 2928810, Name = "Essen", Latitude = 51.45657m, Longitude = 7.01228m, CountryCode = "DE" };
                yield return new City { Id = 2934246, Name = "Düsseldorf", Latitude = 51.22172m, Longitude = 6.77616m, CountryCode = "DE" };
                yield return new City { Id = 2934691, Name = "Duisburg", Latitude = 51.43247m, Longitude = 6.76516m, CountryCode = "DE" };
                yield return new City { Id = 2935517, Name = "Dortmund", Latitude = 51.51494m, Longitude = 7.466m, CountryCode = "DE" };
                yield return new City { Id = 2944388, Name = "Bremen", Latitude = 53.07516m, Longitude = 8.80777m, CountryCode = "DE" };
                yield return new City { Id = 2950159, Name = "Berlin", Latitude = 52.52437m, Longitude = 13.41053m, CountryCode = "DE" };
                yield return new City { Id = 223817, Name = "Djibouti", Latitude = 11.58901m, Longitude = 43.14503m, CountryCode = "DJ" };
                yield return new City { Id = 2618425, Name = "Copenhagen", Latitude = 55.67594m, Longitude = 12.56553m, CountryCode = "DK" };
                yield return new City { Id = 3575635, Name = "Roseau", Latitude = 15.30174m, Longitude = -61.38808m, CountryCode = "DM" };
                yield return new City { Id = 3492908, Name = "Santo Domingo", Latitude = 18.50012m, Longitude = -69.98857m, CountryCode = "DO" };
                yield return new City { Id = 3492914, Name = "Santiago de los Caballeros", Latitude = 19.4517m, Longitude = -70.69703m, CountryCode = "DO" };
                yield return new City { Id = 3493081, Name = "San Juan", Latitude = 18.80588m, Longitude = -71.22991m, CountryCode = "DO" };
                yield return new City { Id = 7874116, Name = "Santo Domingo Oeste", Latitude = 18.5m, Longitude = -70m, CountryCode = "DO" };
                yield return new City { Id = 8601412, Name = "Santo Domingo Este", Latitude = 18.48847m, Longitude = -69.85707m, CountryCode = "DO" };
                yield return new City { Id = 2474141, Name = "Boumerdas", Latitude = 36.76639m, Longitude = 3.47717m, CountryCode = "DZ" };
                yield return new City { Id = 2477461, Name = "Tébessa", Latitude = 35.40417m, Longitude = 8.12417m, CountryCode = "DZ" };
                yield return new City { Id = 2485926, Name = "Oran", Latitude = 35.69111m, Longitude = -0.64167m, CountryCode = "DZ" };
                yield return new City { Id = 2507480, Name = "Algiers", Latitude = 36.7525m, Longitude = 3.04197m, CountryCode = "DZ" };
                yield return new City { Id = 3650960, Name = "Sucre", Latitude = -1.27974m, Longitude = -80.41885m, CountryCode = "EC" };
                yield return new City { Id = 3652462, Name = "Quito", Latitude = -0.22985m, Longitude = -78.52495m, CountryCode = "EC" };
                yield return new City { Id = 3657509, Name = "Guayaquil", Latitude = -2.20584m, Longitude = -79.90795m, CountryCode = "EC" };
                yield return new City { Id = 3660401, Name = "Sucre", Latitude = -0.59792m, Longitude = -80.42367m, CountryCode = "EC" };
                yield return new City { Id = 588409, Name = "Tallinn", Latitude = 59.43696m, Longitude = 24.75353m, CountryCode = "EE" };
                yield return new City { Id = 358619, Name = "Port Said", Latitude = 31.25654m, Longitude = 32.28411m, CountryCode = "EG" };
                yield return new City { Id = 360630, Name = "Cairo", Latitude = 30.06263m, Longitude = 31.24967m, CountryCode = "EG" };
                yield return new City { Id = 360995, Name = "Giza", Latitude = 30.00808m, Longitude = 31.21093m, CountryCode = "EG" };
                yield return new City { Id = 361058, Name = "Alexandria", Latitude = 31.21564m, Longitude = 29.95527m, CountryCode = "EG" };
                yield return new City { Id = 2462881, Name = "El Aaiún", Latitude = 27.1418m, Longitude = -13.18797m, CountryCode = "EH" };
                yield return new City { Id = 343300, Name = "Asmara", Latitude = 15.33805m, Longitude = 38.93184m, CountryCode = "ER" };
                yield return new City { Id = 2509954, Name = "Valencia", Latitude = 39.46975m, Longitude = -0.37739m, CountryCode = "ES" };
                yield return new City { Id = 2510911, Name = "Sevilla", Latitude = 37.38283m, Longitude = -5.97317m, CountryCode = "ES" };
                yield return new City { Id = 2514256, Name = "Málaga", Latitude = 36.72016m, Longitude = -4.42034m, CountryCode = "ES" };
                yield return new City { Id = 3104324, Name = "Zaragoza", Latitude = 41.65606m, Longitude = -0.87734m, CountryCode = "ES" };
                yield return new City { Id = 3113035, Name = "San Juan", Latitude = 42.44775m, Longitude = -8.68594m, CountryCode = "ES" };
                yield return new City { Id = 3117735, Name = "Madrid", Latitude = 40.4165m, Longitude = -3.70256m, CountryCode = "ES" };
                yield return new City { Id = 3128760, Name = "Barcelona", Latitude = 41.38879m, Longitude = 2.15899m, CountryCode = "ES" };
                yield return new City { Id = 344979, Name = "Addis Ababa", Latitude = 9.02497m, Longitude = 38.74689m, CountryCode = "ET" };
                yield return new City { Id = 658225, Name = "Helsinki", Latitude = 60.16952m, Longitude = 24.93545m, CountryCode = "FI" };
                yield return new City { Id = 2198148, Name = "Suva", Latitude = -18.14161m, Longitude = 178.44149m, CountryCode = "FJ" };
                yield return new City { Id = 3426691, Name = "Stanley", Latitude = -51.7m, Longitude = -57.85m, CountryCode = "FK" };
                yield return new City { Id = 2081986, Name = "Palikir", Latitude = 6.92477m, Longitude = 158.16109m, CountryCode = "FM" };
                yield return new City { Id = 2611396, Name = "Tórshavn", Latitude = 62.00973m, Longitude = -6.77164m, CountryCode = "FO" };
                yield return new City { Id = 2977491, Name = "Saint-Pierre", Latitude = 47.38623m, Longitude = 0.74849m, CountryCode = "FR" };
                yield return new City { Id = 2980916, Name = "Saint-Denis", Latitude = 48.93333m, Longitude = 2.36667m, CountryCode = "FR" };
                yield return new City { Id = 2988507, Name = "Paris", Latitude = 48.85341m, Longitude = 2.3488m, CountryCode = "FR" };
                yield return new City { Id = 2995469, Name = "Marseille", Latitude = 43.29695m, Longitude = 5.38107m, CountryCode = "FR" };
                yield return new City { Id = 2399697, Name = "Libreville", Latitude = 0.39241m, Longitude = 9.45356m, CountryCode = "GA" };
                yield return new City { Id = 2634573, Name = "Wellington", Latitude = 52.7m, Longitude = -2.51667m, CountryCode = "GB" };
                yield return new City { Id = 2640194, Name = "Plymouth", Latitude = 50.37153m, Longitude = -4.14305m, CountryCode = "GB" };
                yield return new City { Id = 2643741, Name = "City of London", Latitude = 51.51279m, Longitude = -0.09184m, CountryCode = "GB" };
                yield return new City { Id = 2643743, Name = "London", Latitude = 51.50853m, Longitude = -0.12574m, CountryCode = "GB" };
                yield return new City { Id = 2648579, Name = "Glasgow", Latitude = 55.86515m, Longitude = -4.25763m, CountryCode = "GB" };
                yield return new City { Id = 2655603, Name = "Birmingham", Latitude = 52.48142m, Longitude = -1.89983m, CountryCode = "GB" };
                yield return new City { Id = 8224783, Name = "Stanley", Latitude = 54.86796m, Longitude = -1.69846m, CountryCode = "GB" };
                yield return new City { Id = 3579925, Name = "Georgetown", Latitude = 12.05644m, Longitude = -61.74849m, CountryCode = "GD" };
                yield return new City { Id = 611717, Name = "Tbilisi", Latitude = 41.69411m, Longitude = 44.83368m, CountryCode = "GE" };
                yield return new City { Id = 3382160, Name = "Cayenne", Latitude = 4.93333m, Longitude = -52.33333m, CountryCode = "GF" };
                yield return new City { Id = 3042287, Name = "St. Peter Port", Latitude = 49.45981m, Longitude = -2.53527m, CountryCode = "GG" };
                yield return new City { Id = 2298890, Name = "Kumasi", Latitude = 6.68848m, Longitude = -1.62443m, CountryCode = "GH" };
                yield return new City { Id = 2306104, Name = "Accra", Latitude = 5.55602m, Longitude = -0.1969m, CountryCode = "GH" };
                yield return new City { Id = 2411585, Name = "Gibraltar", Latitude = 36.14474m, Longitude = -5.35257m, CountryCode = "GI" };
                yield return new City { Id = 3421319, Name = "Nuuk", Latitude = 64.18347m, Longitude = -51.72157m, CountryCode = "GL" };
                yield return new City { Id = 2413876, Name = "Banjul", Latitude = 13.45274m, Longitude = -16.57803m, CountryCode = "GM" };
                yield return new City { Id = 2422465, Name = "Conakry", Latitude = 9.53795m, Longitude = -13.67729m, CountryCode = "GN" };
                yield return new City { Id = 2422488, Name = "Camayenne", Latitude = 9.535m, Longitude = -13.68778m, CountryCode = "GN" };
                yield return new City { Id = 3579732, Name = "Basse-Terre", Latitude = 15.99854m, Longitude = -61.72548m, CountryCode = "GP" };
                yield return new City { Id = 2309527, Name = "Malabo", Latitude = 3.75m, Longitude = 8.78333m, CountryCode = "GQ" };
                yield return new City { Id = 264371, Name = "Athens", Latitude = 37.97945m, Longitude = 23.71622m, CountryCode = "GR" };
                yield return new City { Id = 3591060, Name = "San José", Latitude = 13.9274m, Longitude = -90.82166m, CountryCode = "GT" };
                yield return new City { Id = 3598132, Name = "Guatemala City", Latitude = 14.64072m, Longitude = -90.51327m, CountryCode = "GT" };
                yield return new City { Id = 4044012, Name = "Hagåtña", Latitude = 13.47567m, Longitude = 144.74886m, CountryCode = "GU" };
                yield return new City { Id = 2374775, Name = "Bissau", Latitude = 11.86357m, Longitude = -15.59767m, CountryCode = "GW" };
                yield return new City { Id = 3378644, Name = "Georgetown", Latitude = 6.80448m, Longitude = -58.15527m, CountryCode = "GY" };
                yield return new City { Id = 1819609, Name = "Kowloon", Latitude = 22.31667m, Longitude = 114.18333m, CountryCode = "HK" };
                yield return new City { Id = 1819729, Name = "Hong Kong", Latitude = 22.28552m, Longitude = 114.15769m, CountryCode = "HK" };
                yield return new City { Id = 3600949, Name = "Tegucigalpa", Latitude = 14.0818m, Longitude = -87.20681m, CountryCode = "HN" };
                yield return new City { Id = 3186886, Name = "Zagreb", Latitude = 45.81444m, Longitude = 15.97798m, CountryCode = "HR" };
                yield return new City { Id = 3718426, Name = "Port-au-Prince", Latitude = 18.53917m, Longitude = -72.335m, CountryCode = "HT" };
                yield return new City { Id = 3054643, Name = "Budapest", Latitude = 47.49801m, Longitude = 19.03991m, CountryCode = "HU" };
                yield return new City { Id = 1214520, Name = "Medan", Latitude = 3.58333m, Longitude = 98.66667m, CountryCode = "ID" };
                yield return new City { Id = 1621177, Name = "Yogyakarta", Latitude = -7.80139m, Longitude = 110.36472m, CountryCode = "ID" };
                yield return new City { Id = 1622786, Name = "Makassar", Latitude = -5.14861m, Longitude = 119.43194m, CountryCode = "ID" };
                yield return new City { Id = 1624917, Name = "Bandar Lampung", Latitude = -5.42917m, Longitude = 105.26111m, CountryCode = "ID" };
                yield return new City { Id = 1625084, Name = "Tangerang", Latitude = -6.17806m, Longitude = 106.63m, CountryCode = "ID" };
                yield return new City { Id = 1625812, Name = "Surakarta", Latitude = -7.55611m, Longitude = 110.83167m, CountryCode = "ID" };
                yield return new City { Id = 1625822, Name = "Surabaya", Latitude = -7.24917m, Longitude = 112.75083m, CountryCode = "ID" };
                yield return new City { Id = 1626801, Name = "Situbondo", Latitude = -7.70623m, Longitude = 114.00976m, CountryCode = "ID" };
                yield return new City { Id = 1627896, Name = "Semarang", Latitude = -6.99306m, Longitude = 110.42083m, CountryCode = "ID" };
                yield return new City { Id = 1631761, Name = "Pekanbaru", Latitude = 0.51667m, Longitude = 101.44167m, CountryCode = "ID" };
                yield return new City { Id = 1633070, Name = "Palembang", Latitude = -2.91673m, Longitude = 104.7458m, CountryCode = "ID" };
                yield return new City { Id = 1633419, Name = "Padang", Latitude = -0.94924m, Longitude = 100.35427m, CountryCode = "ID" };
                yield return new City { Id = 1636722, Name = "Malang", Latitude = -7.9797m, Longitude = 112.6304m, CountryCode = "ID" };
                yield return new City { Id = 1642911, Name = "Jakarta", Latitude = -6.21462m, Longitude = 106.84513m, CountryCode = "ID" };
                yield return new City { Id = 1645524, Name = "Depok", Latitude = -6.4m, Longitude = 106.81861m, CountryCode = "ID" };
                yield return new City { Id = 1645528, Name = "Denpasar", Latitude = -8.65m, Longitude = 115.21667m, CountryCode = "ID" };
                yield return new City { Id = 1648473, Name = "Bogor", Latitude = -6.59444m, Longitude = 106.78917m, CountryCode = "ID" };
                yield return new City { Id = 1649378, Name = "Bekasi", Latitude = -6.2349m, Longitude = 106.9896m, CountryCode = "ID" };
                yield return new City { Id = 1650213, Name = "Banjarmasin", Latitude = -3.31987m, Longitude = 114.59075m, CountryCode = "ID" };
                yield return new City { Id = 1650357, Name = "Bandung", Latitude = -6.92222m, Longitude = 107.60694m, CountryCode = "ID" };
                yield return new City { Id = 8224624, Name = "City of Balikpapan", Latitude = -1.24204m, Longitude = 116.89419m, CountryCode = "ID" };
                yield return new City { Id = 8581443, Name = "South Tangerang", Latitude = -6.28862m, Longitude = 106.71789m, CountryCode = "ID" };
                yield return new City { Id = 2964506, Name = "Kingstown", Latitude = 53.29395m, Longitude = -6.13586m, CountryCode = "IE" };
                yield return new City { Id = 2964574, Name = "Dublin", Latitude = 53.33306m, Longitude = -6.24889m, CountryCode = "IE" };
                yield return new City { Id = 281184, Name = "Jerusalem", Latitude = 31.76904m, Longitude = 35.21633m, CountryCode = "IL" };
                yield return new City { Id = 3042237, Name = "Douglas", Latitude = 54.15m, Longitude = -4.48333m, CountryCode = "IM" };
                yield return new City { Id = 1252887, Name = "Wellington", Latitude = 11.36552m, Longitude = 76.78442m, CountryCode = "IN" };
                yield return new City { Id = 1252948, Name = "Warangal", Latitude = 18m, Longitude = 79.58333m, CountryCode = "IN" };
                yield return new City { Id = 1253102, Name = "Visakhapatnam", Latitude = 17.68009m, Longitude = 83.20161m, CountryCode = "IN" };
                yield return new City { Id = 1253184, Name = "Vijayawada", Latitude = 16.50745m, Longitude = 80.6466m, CountryCode = "IN" };
                yield return new City { Id = 1253405, Name = "Varanasi", Latitude = 25.31668m, Longitude = 83.01041m, CountryCode = "IN" };
                yield return new City { Id = 1253573, Name = "Vadodara", Latitude = 22.29941m, Longitude = 73.20812m, CountryCode = "IN" };
                yield return new City { Id = 1253894, Name = "Ulhasnagar", Latitude = 19.21667m, Longitude = 73.15m, CountryCode = "IN" };
                yield return new City { Id = 1254163, Name = "Thiruvananthapuram", Latitude = 8.4855m, Longitude = 76.94924m, CountryCode = "IN" };
                yield return new City { Id = 1254388, Name = "Tiruchirappalli", Latitude = 10.8155m, Longitude = 78.69651m, CountryCode = "IN" };
                yield return new City { Id = 1254661, Name = "Thāne", Latitude = 19.19704m, Longitude = 72.96355m, CountryCode = "IN" };
                yield return new City { Id = 1254745, Name = "Teni", Latitude = 10.01531m, Longitude = 77.482m, CountryCode = "IN" };
                yield return new City { Id = 1255364, Name = "Sūrat", Latitude = 21.19594m, Longitude = 72.83023m, CountryCode = "IN" };
                yield return new City { Id = 1255634, Name = "Srinagar", Latitude = 34.08565m, Longitude = 74.80555m, CountryCode = "IN" };
                yield return new City { Id = 1256436, Name = "Solāpur", Latitude = 17.67152m, Longitude = 75.91044m, CountryCode = "IN" };
                yield return new City { Id = 1256525, Name = "Shiliguri", Latitude = 26.71004m, Longitude = 88.42851m, CountryCode = "IN" };
                yield return new City { Id = 1257416, Name = "Sāngli", Latitude = 16.85438m, Longitude = 74.56417m, CountryCode = "IN" };
                yield return new City { Id = 1257629, Name = "Salem", Latitude = 11.65117m, Longitude = 78.15867m, CountryCode = "IN" };
                yield return new City { Id = 1258526, Name = "Ranchi", Latitude = 23.34316m, Longitude = 85.3094m, CountryCode = "IN" };
                yield return new City { Id = 1258847, Name = "Rājkot", Latitude = 22.29161m, Longitude = 70.79322m, CountryCode = "IN" };
                yield return new City { Id = 1258980, Name = "Raipur", Latitude = 21.23333m, Longitude = 81.63333m, CountryCode = "IN" };
                yield return new City { Id = 1259229, Name = "Pune", Latitude = 18.51957m, Longitude = 73.85535m, CountryCode = "IN" };
                yield return new City { Id = 1259652, Name = "Pimpri", Latitude = 18.62292m, Longitude = 73.80696m, CountryCode = "IN" };
                yield return new City { Id = 1260086, Name = "Patna", Latitude = 25.59408m, Longitude = 85.13563m, CountryCode = "IN" };
                yield return new City { Id = 1261162, Name = "Nowrangapur", Latitude = 19.23114m, Longitude = 82.54826m, CountryCode = "IN" };
                yield return new City { Id = 1261481, Name = "New Delhi", Latitude = 28.63576m, Longitude = 77.22445m, CountryCode = "IN" };
                yield return new City { Id = 1261731, Name = "Nashik", Latitude = 19.99727m, Longitude = 73.79096m, CountryCode = "IN" };
                yield return new City { Id = 1261977, Name = "Nanded", Latitude = 19.16023m, Longitude = 77.31497m, CountryCode = "IN" };
                yield return new City { Id = 1262180, Name = "Nagpur", Latitude = 21.14631m, Longitude = 79.08491m, CountryCode = "IN" };
                yield return new City { Id = 1262321, Name = "Mysore", Latitude = 12.29791m, Longitude = 76.63925m, CountryCode = "IN" };
                yield return new City { Id = 1262801, Name = "Morādābād", Latitude = 28.83893m, Longitude = 78.77684m, CountryCode = "IN" };
                yield return new City { Id = 1263214, Name = "Meerut", Latitude = 28.98002m, Longitude = 77.70636m, CountryCode = "IN" };
                yield return new City { Id = 1264521, Name = "Madurai", Latitude = 9.91735m, Longitude = 78.11962m, CountryCode = "IN" };
                yield return new City { Id = 1264527, Name = "Chennai", Latitude = 13.08784m, Longitude = 80.27847m, CountryCode = "IN" };
                yield return new City { Id = 1264728, Name = "Ludhiāna", Latitude = 30.91204m, Longitude = 75.85379m, CountryCode = "IN" };
                yield return new City { Id = 1264733, Name = "Lucknow", Latitude = 26.83928m, Longitude = 80.92313m, CountryCode = "IN" };
                yield return new City { Id = 1266049, Name = "Kota", Latitude = 25.18254m, Longitude = 75.83907m, CountryCode = "IN" };
                yield return new City { Id = 1266285, Name = "Kolhāpur", Latitude = 16.69563m, Longitude = 74.23167m, CountryCode = "IN" };
                yield return new City { Id = 1267696, Name = "Karol Bāgh", Latitude = 28.65136m, Longitude = 77.19072m, CountryCode = "IN" };
                yield return new City { Id = 1267995, Name = "Kanpur", Latitude = 26.46523m, Longitude = 80.34975m, CountryCode = "IN" };
                yield return new City { Id = 1268295, Name = "Kalyān", Latitude = 19.2437m, Longitude = 73.13554m, CountryCode = "IN" };
                yield return new City { Id = 1268782, Name = "Jalandhar", Latitude = 31.32556m, Longitude = 75.57917m, CountryCode = "IN" };
                yield return new City { Id = 1268865, Name = "Jodhpur", Latitude = 26.26841m, Longitude = 73.00594m, CountryCode = "IN" };
                yield return new City { Id = 1269300, Name = "Jamshedpur", Latitude = 22.80278m, Longitude = 86.18545m, CountryCode = "IN" };
                yield return new City { Id = 1269515, Name = "Jaipur", Latitude = 26.91962m, Longitude = 75.78781m, CountryCode = "IN" };
                yield return new City { Id = 1269633, Name = "Jabalpur", Latitude = 23.16697m, Longitude = 79.95006m, CountryCode = "IN" };
                yield return new City { Id = 1269743, Name = "Indore", Latitude = 22.71792m, Longitude = 75.8333m, CountryCode = "IN" };
                yield return new City { Id = 1269843, Name = "Hyderabad", Latitude = 17.38405m, Longitude = 78.45636m, CountryCode = "IN" };
                yield return new City { Id = 1269920, Name = "Hubli", Latitude = 15.34776m, Longitude = 75.13378m, CountryCode = "IN" };
                yield return new City { Id = 1270396, Name = "Hāora", Latitude = 22.57688m, Longitude = 88.31857m, CountryCode = "IN" };
                yield return new City { Id = 1270583, Name = "Gwalior", Latitude = 26.22983m, Longitude = 78.17337m, CountryCode = "IN" };
                yield return new City { Id = 1270668, Name = "Guntur", Latitude = 16.29974m, Longitude = 80.45729m, CountryCode = "IN" };
                yield return new City { Id = 1270926, Name = "Gorakhpur", Latitude = 29.44768m, Longitude = 75.67206m, CountryCode = "IN" };
                yield return new City { Id = 1270927, Name = "Gorakhpur", Latitude = 26.76628m, Longitude = 83.36889m, CountryCode = "IN" };
                yield return new City { Id = 1271308, Name = "Ghāziābād", Latitude = 28.66535m, Longitude = 77.43915m, CountryCode = "IN" };
                yield return new City { Id = 1271476, Name = "Guwahati", Latitude = 26.1844m, Longitude = 91.7458m, CountryCode = "IN" };
                yield return new City { Id = 1271951, Name = "Farīdābād", Latitude = 28.41124m, Longitude = 77.31316m, CountryCode = "IN" };
                yield return new City { Id = 1272175, Name = "Durgapur", Latitude = 23.51583m, Longitude = 87.30801m, CountryCode = "IN" };
                yield return new City { Id = 1272423, Name = "Dombivli", Latitude = 19.21667m, Longitude = 73.08333m, CountryCode = "IN" };
                yield return new City { Id = 1273294, Name = "Delhi", Latitude = 28.65195m, Longitude = 77.23149m, CountryCode = "IN" };
                yield return new City { Id = 1273313, Name = "Dehra Dūn", Latitude = 30.32443m, Longitude = 78.03392m, CountryCode = "IN" };
                yield return new City { Id = 1273780, Name = "Cuttack", Latitude = 20.46497m, Longitude = 85.87927m, CountryCode = "IN" };
                yield return new City { Id = 1273865, Name = "Coimbatore", Latitude = 11.00555m, Longitude = 76.96612m, CountryCode = "IN" };
                yield return new City { Id = 1273874, Name = "Cochin", Latitude = 9.93988m, Longitude = 76.26022m, CountryCode = "IN" };
                yield return new City { Id = 1274746, Name = "Chandigarh", Latitude = 30.73629m, Longitude = 76.7884m, CountryCode = "IN" };
                yield return new City { Id = 1275004, Name = "Kolkata", Latitude = 22.56263m, Longitude = 88.36304m, CountryCode = "IN" };
                yield return new City { Id = 1275248, Name = "Borivli", Latitude = 19.23496m, Longitude = 72.85976m, CountryCode = "IN" };
                yield return new City { Id = 1275339, Name = "Mumbai", Latitude = 19.07283m, Longitude = 72.88261m, CountryCode = "IN" };
                yield return new City { Id = 1275441, Name = "Bissau", Latitude = 28.24737m, Longitude = 75.07666m, CountryCode = "IN" };
                yield return new City { Id = 1275610, Name = "Bilimora", Latitude = 20.76957m, Longitude = 72.96134m, CountryCode = "IN" };
                yield return new City { Id = 1275665, Name = "Bīkaner", Latitude = 28.01762m, Longitude = 73.31495m, CountryCode = "IN" };
                yield return new City { Id = 1275817, Name = "Bhubaneshwar", Latitude = 20.27241m, Longitude = 85.83385m, CountryCode = "IN" };
                yield return new City { Id = 1275841, Name = "Bhopal", Latitude = 23.25469m, Longitude = 77.40289m, CountryCode = "IN" };
                yield return new City { Id = 1275901, Name = "Bhiwandi", Latitude = 19.30023m, Longitude = 73.05881m, CountryCode = "IN" };
                yield return new City { Id = 1275971, Name = "Bhilai", Latitude = 21.20919m, Longitude = 81.4285m, CountryCode = "IN" };
                yield return new City { Id = 1276014, Name = "Bhayandar", Latitude = 19.30157m, Longitude = 72.85107m, CountryCode = "IN" };
                yield return new City { Id = 1276032, Name = "Bhavnagar", Latitude = 21.76287m, Longitude = 72.15331m, CountryCode = "IN" };
                yield return new City { Id = 1277013, Name = "Bareilly", Latitude = 28.34702m, Longitude = 79.42193m, CountryCode = "IN" };
                yield return new City { Id = 1277333, Name = "Bengaluru", Latitude = 12.97194m, Longitude = 77.59369m, CountryCode = "IN" };
                yield return new City { Id = 1278149, Name = "Aurangabad", Latitude = 19.87757m, Longitude = 75.34226m, CountryCode = "IN" };
                yield return new City { Id = 1278314, Name = "Āsansol", Latitude = 23.68333m, Longitude = 86.98333m, CountryCode = "IN" };
                yield return new City { Id = 1278710, Name = "Amritsar", Latitude = 31.62234m, Longitude = 74.87534m, CountryCode = "IN" };
                yield return new City { Id = 1278718, Name = "Amrāvati", Latitude = 20.93333m, Longitude = 77.75m, CountryCode = "IN" };
                yield return new City { Id = 1278994, Name = "Allahābād", Latitude = 25.44478m, Longitude = 81.84322m, CountryCode = "IN" };
                yield return new City { Id = 1279017, Name = "Alīgarh", Latitude = 27.88145m, Longitude = 78.07464m, CountryCode = "IN" };
                yield return new City { Id = 1279159, Name = "Ajmer", Latitude = 26.4521m, Longitude = 74.63867m, CountryCode = "IN" };
                yield return new City { Id = 1279233, Name = "Ahmedabad", Latitude = 23.02579m, Longitude = 72.58727m, CountryCode = "IN" };
                yield return new City { Id = 1279259, Name = "Agra", Latitude = 27.18333m, Longitude = 78.01667m, CountryCode = "IN" };
                yield return new City { Id = 6619347, Name = "Navi Mumbai", Latitude = 19.03681m, Longitude = 73.01582m, CountryCode = "IN" };
                yield return new City { Id = 6943660, Name = "Shivaji Nagar", Latitude = 18.53017m, Longitude = 73.85263m, CountryCode = "IN" };
                yield return new City { Id = 94787, Name = "Kirkuk", Latitude = 35.46806m, Longitude = 44.39222m, CountryCode = "IQ" };
                yield return new City { Id = 95446, Name = "Erbil", Latitude = 36.19257m, Longitude = 44.01062m, CountryCode = "IQ" };
                yield return new City { Id = 98182, Name = "Baghdad", Latitude = 33.34058m, Longitude = 44.40088m, CountryCode = "IQ" };
                yield return new City { Id = 98463, Name = "As Sulaymānīyah", Latitude = 35.56496m, Longitude = 45.4329m, CountryCode = "IQ" };
                yield return new City { Id = 99071, Name = "Al Mawşil al Jadīdah", Latitude = 36.33271m, Longitude = 43.10555m, CountryCode = "IQ" };
                yield return new City { Id = 99072, Name = "Mosul", Latitude = 36.335m, Longitude = 43.11889m, CountryCode = "IQ" };
                yield return new City { Id = 99532, Name = "Basrah", Latitude = 30.50852m, Longitude = 47.7804m, CountryCode = "IQ" };
                yield return new City { Id = 100077, Name = "Abū Ghurayb", Latitude = 33.30563m, Longitude = 44.18477m, CountryCode = "IQ" };
                yield return new City { Id = 388349, Name = "Al Başrah al Qadīmah", Latitude = 30.50316m, Longitude = 47.81507m, CountryCode = "IQ" };
                yield return new City { Id = 14256, Name = "Āzādshahr", Latitude = 34.79049m, Longitude = 48.57011m, CountryCode = "IR" };
                yield return new City { Id = 23814, Name = "Kahrīz", Latitude = 34.3838m, Longitude = 47.0553m, CountryCode = "IR" };
                yield return new City { Id = 112931, Name = "Tehran", Latitude = 35.69439m, Longitude = 51.42151m, CountryCode = "IR" };
                yield return new City { Id = 113646, Name = "Tabriz", Latitude = 38.08m, Longitude = 46.2919m, CountryCode = "IR" };
                yield return new City { Id = 115019, Name = "Shiraz", Latitude = 29.61031m, Longitude = 52.53113m, CountryCode = "IR" };
                yield return new City { Id = 118743, Name = "Rasht", Latitude = 37.27611m, Longitude = 49.58862m, CountryCode = "IR" };
                yield return new City { Id = 119208, Name = "Qom", Latitude = 34.6401m, Longitude = 50.8764m, CountryCode = "IR" };
                yield return new City { Id = 121801, Name = "Orūmīyeh", Latitude = 37.55274m, Longitude = 45.07605m, CountryCode = "IR" };
                yield return new City { Id = 124665, Name = "Mashhad", Latitude = 36.31559m, Longitude = 59.56796m, CountryCode = "IR" };
                yield return new City { Id = 128226, Name = "Kermanshah", Latitude = 34.31417m, Longitude = 47.065m, CountryCode = "IR" };
                yield return new City { Id = 128234, Name = "Kerman", Latitude = 30.28321m, Longitude = 57.07879m, CountryCode = "IR" };
                yield return new City { Id = 128747, Name = "Karaj", Latitude = 35.83266m, Longitude = 50.99155m, CountryCode = "IR" };
                yield return new City { Id = 132144, Name = "Hamadān", Latitude = 34.79922m, Longitude = 48.51456m, CountryCode = "IR" };
                yield return new City { Id = 143127, Name = "Arāk", Latitude = 34.09174m, Longitude = 49.68916m, CountryCode = "IR" };
                yield return new City { Id = 144448, Name = "Ahvaz", Latitude = 31.31901m, Longitude = 48.6842m, CountryCode = "IR" };
                yield return new City { Id = 418863, Name = "Isfahan", Latitude = 32.65246m, Longitude = 51.67462m, CountryCode = "IR" };
                yield return new City { Id = 1159301, Name = "Zahedan", Latitude = 29.4963m, Longitude = 60.8629m, CountryCode = "IR" };
                yield return new City { Id = 10630176, Name = "Pasragad Branch", Latitude = 34.77772m, Longitude = 48.47168m, CountryCode = "IR" };
                yield return new City { Id = 3413829, Name = "Reykjavik", Latitude = 64.13548m, Longitude = -21.89541m, CountryCode = "IS" };
                yield return new City { Id = 2523920, Name = "Palermo", Latitude = 38.13205m, Longitude = 13.33561m, CountryCode = "IT" };
                yield return new City { Id = 3165524, Name = "Turin", Latitude = 45.07049m, Longitude = 7.68682m, CountryCode = "IT" };
                yield return new City { Id = 3169070, Name = "Rome", Latitude = 41.89193m, Longitude = 12.51133m, CountryCode = "IT" };
                yield return new City { Id = 3172394, Name = "Naples", Latitude = 40.85631m, Longitude = 14.24641m, CountryCode = "IT" };
                yield return new City { Id = 3173435, Name = "Milan", Latitude = 45.46427m, Longitude = 9.18951m, CountryCode = "IT" };
                yield return new City { Id = 3176219, Name = "Genoa", Latitude = 44.4264m, Longitude = 8.91519m, CountryCode = "IT" };
                yield return new City { Id = 3042091, Name = "Saint Helier", Latitude = 49.18804m, Longitude = -2.10491m, CountryCode = "JE" };
                yield return new City { Id = 3489297, Name = "New Kingston", Latitude = 18.00747m, Longitude = -76.78319m, CountryCode = "JM" };
                yield return new City { Id = 3489854, Name = "Kingston", Latitude = 17.99702m, Longitude = -76.79358m, CountryCode = "JM" };
                yield return new City { Id = 250090, Name = "Zarqa", Latitude = 32.07275m, Longitude = 36.08796m, CountryCode = "JO" };
                yield return new City { Id = 250441, Name = "Amman", Latitude = 31.95522m, Longitude = 35.94503m, CountryCode = "JO" };
                yield return new City { Id = 1848254, Name = "Yono", Latitude = 35.88333m, Longitude = 139.63333m, CountryCode = "JP" };
                yield return new City { Id = 1848354, Name = "Yokohama", Latitude = 35.43333m, Longitude = 139.65m, CountryCode = "JP" };
                yield return new City { Id = 1850147, Name = "Tokyo", Latitude = 35.6895m, Longitude = 139.69171m, CountryCode = "JP" };
                yield return new City { Id = 1851368, Name = "Suva", Latitude = 36.03799m, Longitude = 138.11308m, CountryCode = "JP" };
                yield return new City { Id = 1851717, Name = "Shizuoka", Latitude = 34.98333m, Longitude = 138.38333m, CountryCode = "JP" };
                yield return new City { Id = 1853195, Name = "Sakai", Latitude = 34.58333m, Longitude = 135.46667m, CountryCode = "JP" };
                yield return new City { Id = 1853909, Name = "Osaka", Latitude = 34.69374m, Longitude = 135.50218m, CountryCode = "JP" };
                yield return new City { Id = 1854383, Name = "Okayama", Latitude = 34.65m, Longitude = 133.93333m, CountryCode = "JP" };
                yield return new City { Id = 1855431, Name = "Niigata", Latitude = 37.88637m, Longitude = 139.00589m, CountryCode = "JP" };
                yield return new City { Id = 1856057, Name = "Nagoya", Latitude = 35.18147m, Longitude = 136.90641m, CountryCode = "JP" };
                yield return new City { Id = 1857910, Name = "Kyoto", Latitude = 35.02107m, Longitude = 135.75385m, CountryCode = "JP" };
                yield return new City { Id = 1858421, Name = "Kumamoto", Latitude = 32.80589m, Longitude = 130.69181m, CountryCode = "JP" };
                yield return new City { Id = 1859171, Name = "Kobe", Latitude = 34.6913m, Longitude = 135.183m, CountryCode = "JP" };
                yield return new City { Id = 1859307, Name = "Kitakyushu", Latitude = 33.85181m, Longitude = 130.85034m, CountryCode = "JP" };
                yield return new City { Id = 1859642, Name = "Kawasaki", Latitude = 35.52056m, Longitude = 139.71722m, CountryCode = "JP" };
                yield return new City { Id = 1860827, Name = "Kagoshima", Latitude = 31.56667m, Longitude = 130.55m, CountryCode = "JP" };
                yield return new City { Id = 1862415, Name = "Hiroshima", Latitude = 34.4m, Longitude = 132.45m, CountryCode = "JP" };
                yield return new City { Id = 1863289, Name = "Hamamatsu", Latitude = 34.7m, Longitude = 137.73333m, CountryCode = "JP" };
                yield return new City { Id = 1863440, Name = "Hachiōji", Latitude = 35.65583m, Longitude = 139.32389m, CountryCode = "JP" };
                yield return new City { Id = 1863905, Name = "Honchō", Latitude = 35.70129m, Longitude = 139.98648m, CountryCode = "JP" };
                yield return new City { Id = 1863967, Name = "Fukuoka", Latitude = 33.6m, Longitude = 130.41667m, CountryCode = "JP" };
                yield return new City { Id = 2111149, Name = "Sendai", Latitude = 38.26667m, Longitude = 140.86667m, CountryCode = "JP" };
                yield return new City { Id = 2113015, Name = "Chiba", Latitude = 35.6m, Longitude = 140.11667m, CountryCode = "JP" };
                yield return new City { Id = 2128295, Name = "Sapporo", Latitude = 43.06667m, Longitude = 141.35m, CountryCode = "JP" };
                yield return new City { Id = 6940394, Name = "Saitama", Latitude = 35.90807m, Longitude = 139.65657m, CountryCode = "JP" };
                yield return new City { Id = 184745, Name = "Nairobi", Latitude = -1.28333m, Longitude = 36.81667m, CountryCode = "KE" };
                yield return new City { Id = 186301, Name = "Mombasa", Latitude = -4.05466m, Longitude = 39.66359m, CountryCode = "KE" };
                yield return new City { Id = 1528675, Name = "Bishkek", Latitude = 42.87m, Longitude = 74.59m, CountryCode = "KG" };
                yield return new City { Id = 1821306, Name = "Phnom Penh", Latitude = 11.56245m, Longitude = 104.91601m, CountryCode = "KH" };
                yield return new City { Id = 1821940, Name = "Takeo", Latitude = 10.99081m, Longitude = 104.78498m, CountryCode = "KH" };
                yield return new City { Id = 2110257, Name = "South Tarawa", Latitude = 1.3278m, Longitude = 172.97696m, CountryCode = "KI" };
                yield return new City { Id = 921772, Name = "Moroni", Latitude = -11.70216m, Longitude = 43.25506m, CountryCode = "KM" };
                yield return new City { Id = 3575551, Name = "Basseterre", Latitude = 17.29484m, Longitude = -62.7261m, CountryCode = "KN" };
                yield return new City { Id = 1871859, Name = "Pyongyang", Latitude = 39.03385m, Longitude = 125.75432m, CountryCode = "KP" };
                yield return new City { Id = 1877449, Name = "Hamhŭng", Latitude = 39.91833m, Longitude = 127.53639m, CountryCode = "KP" };
                yield return new City { Id = 1833747, Name = "Ulsan", Latitude = 35.53722m, Longitude = 129.31667m, CountryCode = "KR" };
                yield return new City { Id = 1835235, Name = "Daejeon", Latitude = 36.32139m, Longitude = 127.41972m, CountryCode = "KR" };
                yield return new City { Id = 1835329, Name = "Daegu", Latitude = 35.87028m, Longitude = 128.59111m, CountryCode = "KR" };
                yield return new City { Id = 1835553, Name = "Suwon-si", Latitude = 37.29111m, Longitude = 127.00889m, CountryCode = "KR" };
                yield return new City { Id = 1835848, Name = "Seoul", Latitude = 37.566m, Longitude = 126.9784m, CountryCode = "KR" };
                yield return new City { Id = 1838524, Name = "Busan", Latitude = 35.10278m, Longitude = 129.04028m, CountryCode = "KR" };
                yield return new City { Id = 1838716, Name = "Bucheon-si", Latitude = 37.49889m, Longitude = 126.78306m, CountryCode = "KR" };
                yield return new City { Id = 1841811, Name = "Gwangju", Latitude = 35.15472m, Longitude = 126.91556m, CountryCode = "KR" };
                yield return new City { Id = 1842485, Name = "Goyang-si", Latitude = 37.65639m, Longitude = 126.835m, CountryCode = "KR" };
                yield return new City { Id = 1843564, Name = "Incheon", Latitude = 37.45646m, Longitude = 126.70515m, CountryCode = "KR" };
                yield return new City { Id = 1845457, Name = "Jeonju", Latitude = 35.82194m, Longitude = 127.14889m, CountryCode = "KR" };
                yield return new City { Id = 1845604, Name = "Cheongju-si", Latitude = 36.63722m, Longitude = 127.48972m, CountryCode = "KR" };
                yield return new City { Id = 1846326, Name = "Changwon", Latitude = 35.22806m, Longitude = 128.68111m, CountryCode = "KR" };
                yield return new City { Id = 1846898, Name = "Anyang-si", Latitude = 37.3925m, Longitude = 126.92694m, CountryCode = "KR" };
                yield return new City { Id = 1846918, Name = "Ansan-si", Latitude = 37.32361m, Longitude = 126.82194m, CountryCode = "KR" };
                yield return new City { Id = 1897000, Name = "Seongnam-si", Latitude = 37.43861m, Longitude = 127.13778m, CountryCode = "KR" };
                yield return new City { Id = 285787, Name = "Kuwait City", Latitude = 29.36972m, Longitude = 47.97833m, CountryCode = "KW" };
                yield return new City { Id = 285839, Name = "Al Aḩmadī", Latitude = 29.07694m, Longitude = 48.08389m, CountryCode = "KW" };
                yield return new City { Id = 3580661, Name = "Georgetown", Latitude = 19.2866m, Longitude = -81.37436m, CountryCode = "KY" };
                yield return new City { Id = 1526273, Name = "Astana", Latitude = 51.1801m, Longitude = 71.44598m, CountryCode = "KZ" };
                yield return new City { Id = 1526384, Name = "Almaty", Latitude = 43.25667m, Longitude = 76.92861m, CountryCode = "KZ" };
                yield return new City { Id = 1651944, Name = "Vientiane", Latitude = 17.96667m, Longitude = 102.6m, CountryCode = "LA" };
                yield return new City { Id = 268743, Name = "Ra’s Bayrūt", Latitude = 33.9m, Longitude = 35.48333m, CountryCode = "LB" };
                yield return new City { Id = 276781, Name = "Beirut", Latitude = 33.88894m, Longitude = 35.49442m, CountryCode = "LB" };
                yield return new City { Id = 3576812, Name = "Castries", Latitude = 13.9957m, Longitude = -61.00614m, CountryCode = "LC" };
                yield return new City { Id = 3042030, Name = "Vaduz", Latitude = 47.14151m, Longitude = 9.52154m, CountryCode = "LI" };
                yield return new City { Id = 1248991, Name = "Colombo", Latitude = 6.93194m, Longitude = 79.84778m, CountryCode = "LK" };
                yield return new City { Id = 2274895, Name = "Monrovia", Latitude = 6.30054m, Longitude = -10.7969m, CountryCode = "LR" };
                yield return new City { Id = 932505, Name = "Maseru", Latitude = -29.31667m, Longitude = 27.48333m, CountryCode = "LS" };
                yield return new City { Id = 593116, Name = "Vilnius", Latitude = 54.68916m, Longitude = 25.2798m, CountryCode = "LT" };
                yield return new City { Id = 2960316, Name = "Luxembourg", Latitude = 49.61167m, Longitude = 6.13m, CountryCode = "LU" };
                yield return new City { Id = 456172, Name = "Riga", Latitude = 56.946m, Longitude = 24.10589m, CountryCode = "LV" };
                yield return new City { Id = 88319, Name = "Benghazi", Latitude = 32.11486m, Longitude = 20.06859m, CountryCode = "LY" };
                yield return new City { Id = 2210247, Name = "Tripoli", Latitude = 32.87519m, Longitude = 13.18746m, CountryCode = "LY" };
                yield return new City { Id = 2530335, Name = "Tangier", Latitude = 35.76727m, Longitude = -5.79975m, CountryCode = "MA" };
                yield return new City { Id = 2537763, Name = "Sale", Latitude = 34.0531m, Longitude = -6.79846m, CountryCode = "MA" };
                yield return new City { Id = 2538475, Name = "Rabat", Latitude = 34.01325m, Longitude = -6.83255m, CountryCode = "MA" };
                yield return new City { Id = 2542715, Name = "Meknès", Latitude = 33.89352m, Longitude = -5.54727m, CountryCode = "MA" };
                yield return new City { Id = 2542997, Name = "Marrakesh", Latitude = 31.63416m, Longitude = -7.99994m, CountryCode = "MA" };
                yield return new City { Id = 2548885, Name = "Fès", Latitude = 34.03313m, Longitude = -5.00028m, CountryCode = "MA" };
                yield return new City { Id = 2553604, Name = "Casablanca", Latitude = 33.58831m, Longitude = -7.61138m, CountryCode = "MA" };
                yield return new City { Id = 2561668, Name = "Agadir", Latitude = 30.42018m, Longitude = -9.59815m, CountryCode = "MA" };
                yield return new City { Id = 2993458, Name = "Monaco", Latitude = 43.73333m, Longitude = 7.41667m, CountryCode = "MC" };
                yield return new City { Id = 618426, Name = "Chisinau", Latitude = 47.00556m, Longitude = 28.8575m, CountryCode = "MD" };
                yield return new City { Id = 3193044, Name = "Podgorica", Latitude = 42.44111m, Longitude = 19.26361m, CountryCode = "ME" };
                yield return new City { Id = 3578851, Name = "Marigot", Latitude = 18.06819m, Longitude = -63.08302m, CountryCode = "MF" };
                yield return new City { Id = 1070940, Name = "Antananarivo", Latitude = -18.91368m, Longitude = 47.53613m, CountryCode = "MG" };
                yield return new City { Id = 2113779, Name = "Majuro", Latitude = 7.08971m, Longitude = 171.38027m, CountryCode = "MH" };
                yield return new City { Id = 785842, Name = "Skopje", Latitude = 41.99646m, Longitude = 21.43141m, CountryCode = "MK" };
                yield return new City { Id = 2460596, Name = "Bamako", Latitude = 12.65m, Longitude = -8m, CountryCode = "ML" };
                yield return new City { Id = 1298824, Name = "Yangon", Latitude = 16.80528m, Longitude = 96.15611m, CountryCode = "MM" };
                yield return new City { Id = 1311874, Name = "Mandalay", Latitude = 21.97473m, Longitude = 96.08359m, CountryCode = "MM" };
                yield return new City { Id = 6611854, Name = "Nay Pyi Taw", Latitude = 19.745m, Longitude = 96.12972m, CountryCode = "MM" };
                yield return new City { Id = 2028462, Name = "Ulan Bator", Latitude = 47.90771m, Longitude = 106.88324m, CountryCode = "MN" };
                yield return new City { Id = 1821274, Name = "Macau", Latitude = 22.20056m, Longitude = 113.54611m, CountryCode = "MO" };
                yield return new City { Id = 7828758, Name = "Saipan", Latitude = 15.21233m, Longitude = 145.7545m, CountryCode = "MP" };
                yield return new City { Id = 3570675, Name = "Fort-de-France", Latitude = 14.60892m, Longitude = -61.07334m, CountryCode = "MQ" };
                yield return new City { Id = 2377450, Name = "Nouakchott", Latitude = 18.08581m, Longitude = -15.9785m, CountryCode = "MR" };
                yield return new City { Id = 3578069, Name = "Plymouth", Latitude = 16.70555m, Longitude = -62.21292m, CountryCode = "MS" };
                yield return new City { Id = 2562305, Name = "Valletta", Latitude = 35.89972m, Longitude = 14.51472m, CountryCode = "MT" };
                yield return new City { Id = 934154, Name = "Port Louis", Latitude = -20.16194m, Longitude = 57.49889m, CountryCode = "MU" };
                yield return new City { Id = 1282027, Name = "Malé", Latitude = 4.1748m, Longitude = 73.50888m, CountryCode = "MV" };
                yield return new City { Id = 927967, Name = "Lilongwe", Latitude = -13.96692m, Longitude = 33.78725m, CountryCode = "MW" };
                yield return new City { Id = 931755, Name = "Blantyre", Latitude = -15.78499m, Longitude = 35.00854m, CountryCode = "MW" };
                yield return new City { Id = 3514663, Name = "Álvaro Obregón", Latitude = 19.35867m, Longitude = -99.20329m, CountryCode = "MX" };
                yield return new City { Id = 3514674, Name = "Gustavo A. Madero", Latitude = 19.49016m, Longitude = -99.10978m, CountryCode = "MX" };
                yield return new City { Id = 3514783, Name = "Veracruz", Latitude = 19.18095m, Longitude = -96.1429m, CountryCode = "MX" };
                yield return new City { Id = 3515302, Name = "Toluca", Latitude = 19.28786m, Longitude = -99.65324m, CountryCode = "MX" };
                yield return new City { Id = 3515428, Name = "Tlalpan", Latitude = 19.29707m, Longitude = -99.16787m, CountryCode = "MX" };
                yield return new City { Id = 3515431, Name = "Tlalnepantla", Latitude = 19.54005m, Longitude = -99.19538m, CountryCode = "MX" };
                yield return new City { Id = 3517270, Name = "Santa María Chimalhuacán", Latitude = 19.42155m, Longitude = -98.95038m, CountryCode = "MX" };
                yield return new City { Id = 3518618, Name = "San Juan", Latitude = 19.68862m, Longitude = -98.8611m, CountryCode = "MX" };
                yield return new City { Id = 3521081, Name = "Puebla", Latitude = 19.03793m, Longitude = -98.20346m, CountryCode = "MX" };
                yield return new City { Id = 3522790, Name = "Naucalpan de Juárez", Latitude = 19.47851m, Longitude = -99.23963m, CountryCode = "MX" };
                yield return new City { Id = 3523349, Name = "Mérida", Latitude = 20.97537m, Longitude = -89.61696m, CountryCode = "MX" };
                yield return new City { Id = 3526683, Name = "Iztapalapa", Latitude = 19.35738m, Longitude = -99.0671m, CountryCode = "MX" };
                yield return new City { Id = 3529612, Name = "Ecatepec", Latitude = 19.61725m, Longitude = -99.06601m, CountryCode = "MX" };
                yield return new City { Id = 3530139, Name = "Coyoacán", Latitude = 19.3467m, Longitude = -99.16174m, CountryCode = "MX" };
                yield return new City { Id = 3530580, Name = "Victoria", Latitude = 23.74174m, Longitude = -99.14599m, CountryCode = "MX" };
                yield return new City { Id = 3530589, Name = "Ciudad Nezahualcoyotl", Latitude = 19.40061m, Longitude = -99.01483m, CountryCode = "MX" };
                yield return new City { Id = 3530597, Name = "Mexico City", Latitude = 19.42847m, Longitude = -99.12766m, CountryCode = "MX" };
                yield return new City { Id = 3531673, Name = "Cancún", Latitude = 21.17429m, Longitude = -86.84656m, CountryCode = "MX" };
                yield return new City { Id = 3532624, Name = "Ciudad López Mateos", Latitude = 19.58547m, Longitude = -99.26035m, CountryCode = "MX" };
                yield return new City { Id = 3533462, Name = "Acapulco de Juárez", Latitude = 16.86336m, Longitude = -99.8901m, CountryCode = "MX" };
                yield return new City { Id = 3827409, Name = "Cuauhtémoc", Latitude = 19.44506m, Longitude = -99.14612m, CountryCode = "MX" };
                yield return new City { Id = 3979770, Name = "Zapopan", Latitude = 20.72356m, Longitude = -103.38479m, CountryCode = "MX" };
                yield return new City { Id = 3981254, Name = "Torreon", Latitude = 25.54389m, Longitude = -103.41898m, CountryCode = "MX" };
                yield return new City { Id = 3981609, Name = "Tijuana", Latitude = 32.5027m, Longitude = -117.00371m, CountryCode = "MX" };
                yield return new City { Id = 3985606, Name = "San Luis Potosí", Latitude = 22.14982m, Longitude = -100.97916m, CountryCode = "MX" };
                yield return new City { Id = 3988086, Name = "Saltillo", Latitude = 25.42321m, Longitude = -101.0053m, CountryCode = "MX" };
                yield return new City { Id = 3991164, Name = "Santiago de Querétaro", Latitude = 20.58806m, Longitude = -100.38806m, CountryCode = "MX" };
                yield return new City { Id = 3995402, Name = "Morelia", Latitude = 19.70078m, Longitude = -101.18443m, CountryCode = "MX" };
                yield return new City { Id = 3995465, Name = "Monterrey", Latitude = 25.67507m, Longitude = -100.31847m, CountryCode = "MX" };
                yield return new City { Id = 3996069, Name = "Mexicali", Latitude = 32.62781m, Longitude = -115.45446m, CountryCode = "MX" };
                yield return new City { Id = 3998655, Name = "León de los Aldama", Latitude = 21.12908m, Longitude = -101.67374m, CountryCode = "MX" };
                yield return new City { Id = 4004898, Name = "Hermosillo", Latitude = 29.1026m, Longitude = -110.97732m, CountryCode = "MX" };
                yield return new City { Id = 4005492, Name = "Guadalupe", Latitude = 25.67678m, Longitude = -100.25646m, CountryCode = "MX" };
                yield return new City { Id = 4005539, Name = "Guadalajara", Latitude = 20.66682m, Longitude = -103.39182m, CountryCode = "MX" };
                yield return new City { Id = 4012176, Name = "Culiacán", Latitude = 24.79032m, Longitude = -107.38782m, CountryCode = "MX" };
                yield return new City { Id = 4013708, Name = "Ciudad Juárez", Latitude = 31.73333m, Longitude = -106.48333m, CountryCode = "MX" };
                yield return new City { Id = 4014338, Name = "Chihuahua", Latitude = 28.63528m, Longitude = -106.08889m, CountryCode = "MX" };
                yield return new City { Id = 4019233, Name = "Aguascalientes", Latitude = 21.88234m, Longitude = -102.28259m, CountryCode = "MX" };
                yield return new City { Id = 1732752, Name = "Johor Bahru", Latitude = 1.4655m, Longitude = 103.7578m, CountryCode = "MY" };
                yield return new City { Id = 1732905, Name = "Klang", Latitude = 3.03333m, Longitude = 101.45m, CountryCode = "MY" };
                yield return new City { Id = 1733782, Name = "Victoria", Latitude = 5.27667m, Longitude = 115.24167m, CountryCode = "MY" };
                yield return new City { Id = 1734634, Name = "Ipoh", Latitude = 4.5841m, Longitude = 101.0829m, CountryCode = "MY" };
                yield return new City { Id = 1735106, Name = "Georgetown", Latitude = 5.41123m, Longitude = 100.33543m, CountryCode = "MY" };
                yield return new City { Id = 1735158, Name = "Petaling Jaya", Latitude = 3.10726m, Longitude = 101.60671m, CountryCode = "MY" };
                yield return new City { Id = 1735161, Name = "Kuala Lumpur", Latitude = 3.1412m, Longitude = 101.68653m, CountryCode = "MY" };
                yield return new City { Id = 1735634, Name = "Kuching", Latitude = 1.55m, Longitude = 110.33333m, CountryCode = "MY" };
                yield return new City { Id = 1736376, Name = "Kota Bharu", Latitude = 6.13328m, Longitude = 102.2386m, CountryCode = "MY" };
                yield return new City { Id = 1771023, Name = "Kampung Baru Subang", Latitude = 3.15m, Longitude = 101.53333m, CountryCode = "MY" };
                yield return new City { Id = 8504423, Name = "Subang Jaya", Latitude = 3.04384m, Longitude = 101.58062m, CountryCode = "MY" };
                yield return new City { Id = 1039854, Name = "Matola", Latitude = -25.96222m, Longitude = 32.45889m, CountryCode = "MZ" };
                yield return new City { Id = 1040652, Name = "Maputo", Latitude = -25.96553m, Longitude = 32.58322m, CountryCode = "MZ" };
                yield return new City { Id = 1052373, Name = "Beira", Latitude = -19.84361m, Longitude = 34.83889m, CountryCode = "MZ" };
                yield return new City { Id = 3352136, Name = "Windhoek", Latitude = -22.55941m, Longitude = 17.08323m, CountryCode = "NA" };
                yield return new City { Id = 2139521, Name = "Nouméa", Latitude = -22.27631m, Longitude = 166.4572m, CountryCode = "NC" };
                yield return new City { Id = 2440485, Name = "Niamey", Latitude = 13.51366m, Longitude = 2.1098m, CountryCode = "NE" };
                yield return new City { Id = 2317765, Name = "Zaria", Latitude = 11.11128m, Longitude = 7.7227m, CountryCode = "NG" };
                yield return new City { Id = 2319133, Name = "Warri", Latitude = 5.51737m, Longitude = 5.75006m, CountryCode = "NG" };
                yield return new City { Id = 2322911, Name = "Sokoto", Latitude = 13.06269m, Longitude = 5.24322m, CountryCode = "NG" };
                yield return new City { Id = 2324774, Name = "Port Harcourt", Latitude = 4.77742m, Longitude = 7.0134m, CountryCode = "NG" };
                yield return new City { Id = 2325200, Name = "Oyo", Latitude = 7.85257m, Longitude = 3.93125m, CountryCode = "NG" };
                yield return new City { Id = 2326016, Name = "Onitsha", Latitude = 6.14978m, Longitude = 6.78569m, CountryCode = "NG" };
                yield return new City { Id = 2331447, Name = "Maiduguri", Latitude = 11.84692m, Longitude = 13.15712m, CountryCode = "NG" };
                yield return new City { Id = 2332459, Name = "Lagos", Latitude = 6.45407m, Longitude = 3.39467m, CountryCode = "NG" };
                yield return new City { Id = 2335204, Name = "Kano", Latitude = 12.00012m, Longitude = 8.51672m, CountryCode = "NG" };
                yield return new City { Id = 2335727, Name = "Kaduna", Latitude = 10.52641m, Longitude = 7.43879m, CountryCode = "NG" };
                yield return new City { Id = 2335953, Name = "Jos", Latitude = 9.92849m, Longitude = 8.89212m, CountryCode = "NG" };
                yield return new City { Id = 2337639, Name = "Ilorin", Latitude = 8.49664m, Longitude = 4.54214m, CountryCode = "NG" };
                yield return new City { Id = 2339354, Name = "Ibadan", Latitude = 7.37756m, Longitude = 3.90591m, CountryCode = "NG" };
                yield return new City { Id = 2343279, Name = "Enugu", Latitude = 6.44132m, Longitude = 7.49883m, CountryCode = "NG" };
                yield return new City { Id = 2344082, Name = "Ebute Ikorodu", Latitude = 6.60086m, Longitude = 3.48818m, CountryCode = "NG" };
                yield return new City { Id = 2347283, Name = "Benin City", Latitude = 6.33815m, Longitude = 5.62575m, CountryCode = "NG" };
                yield return new City { Id = 2352778, Name = "Abuja", Latitude = 9.05785m, Longitude = 7.49508m, CountryCode = "NG" };
                yield return new City { Id = 2352947, Name = "Abeokuta", Latitude = 7.15571m, Longitude = 3.34509m, CountryCode = "NG" };
                yield return new City { Id = 2353151, Name = "Aba", Latitude = 5.10658m, Longitude = 7.36667m, CountryCode = "NG" };
                yield return new City { Id = 3617763, Name = "Managua", Latitude = 12.13282m, Longitude = -86.2504m, CountryCode = "NI" };
                yield return new City { Id = 2747891, Name = "Rotterdam", Latitude = 51.9225m, Longitude = 4.47917m, CountryCode = "NL" };
                yield return new City { Id = 2759794, Name = "Amsterdam", Latitude = 52.37403m, Longitude = 4.88969m, CountryCode = "NL" };
                yield return new City { Id = 3143244, Name = "Oslo", Latitude = 59.91273m, Longitude = 10.74609m, CountryCode = "NO" };
                yield return new City { Id = 1283240, Name = "Kathmandu", Latitude = 27.70169m, Longitude = 85.3206m, CountryCode = "NP" };
                yield return new City { Id = 7626461, Name = "Yaren", Latitude = -0.55085m, Longitude = 166.9252m, CountryCode = "NR" };
                yield return new City { Id = 4036284, Name = "Alofi", Latitude = -19.05451m, Longitude = -169.91768m, CountryCode = "NU" };
                yield return new City { Id = 2179537, Name = "Wellington", Latitude = -41.28664m, Longitude = 174.77557m, CountryCode = "NZ" };
                yield return new City { Id = 287286, Name = "Muscat", Latitude = 23.58413m, Longitude = 58.40778m, CountryCode = "OM" };
                yield return new City { Id = 3703443, Name = "Panama City", Latitude = 8.9936m, Longitude = -79.51973m, CountryCode = "PA" };
                yield return new City { Id = 3691175, Name = "Trujillo", Latitude = -8.11599m, Longitude = -79.02998m, CountryCode = "PE" };
                yield return new City { Id = 3698350, Name = "Chiclayo", Latitude = -6.77137m, Longitude = -79.84088m, CountryCode = "PE" };
                yield return new City { Id = 3936456, Name = "Lima", Latitude = -12.04318m, Longitude = -77.02824m, CountryCode = "PE" };
                yield return new City { Id = 3946083, Name = "Callao", Latitude = -12.05659m, Longitude = -77.11814m, CountryCode = "PE" };
                yield return new City { Id = 3947322, Name = "Arequipa", Latitude = -16.39889m, Longitude = -71.535m, CountryCode = "PE" };
                yield return new City { Id = 4033936, Name = "Papeete", Latitude = -17.53733m, Longitude = -149.5665m, CountryCode = "PF" };
                yield return new City { Id = 2088122, Name = "Port Moresby", Latitude = -9.44314m, Longitude = 147.17972m, CountryCode = "PG" };
                yield return new City { Id = 1680018, Name = "Victoria", Latitude = 14.2277m, Longitude = 121.3292m, CountryCode = "PH" };
                yield return new City { Id = 1684308, Name = "Taguig", Latitude = 14.5243m, Longitude = 121.0792m, CountryCode = "PH" };
                yield return new City { Id = 1689286, Name = "San Juan", Latitude = 14.6m, Longitude = 121.0333m, CountryCode = "PH" };
                yield return new City { Id = 1692192, Name = "Quezon City", Latitude = 14.6488m, Longitude = 121.0509m, CountryCode = "PH" };
                yield return new City { Id = 1701668, Name = "Manila", Latitude = 14.6042m, Longitude = 120.9822m, CountryCode = "PH" };
                yield return new City { Id = 1703417, Name = "Makati City", Latitude = 14.55027m, Longitude = 121.03269m, CountryCode = "PH" };
                yield return new City { Id = 1713022, Name = "General Santos", Latitude = 6.11278m, Longitude = 125.17167m, CountryCode = "PH" };
                yield return new City { Id = 1715348, Name = "Davao", Latitude = 7.07306m, Longitude = 125.61278m, CountryCode = "PH" };
                yield return new City { Id = 1717512, Name = "Cebu City", Latitude = 10.31672m, Longitude = 123.89071m, CountryCode = "PH" };
                yield return new City { Id = 1723510, Name = "Budta", Latitude = 7.20417m, Longitude = 124.43972m, CountryCode = "PH" };
                yield return new City { Id = 1730501, Name = "Antipolo", Latitude = 14.62578m, Longitude = 121.12251m, CountryCode = "PH" };
                yield return new City { Id = 1978681, Name = "Malingao", Latitude = 7.16083m, Longitude = 124.475m, CountryCode = "PH" };
                yield return new City { Id = 7290466, Name = "Pasig City", Latitude = 14.58691m, Longitude = 121.0614m, CountryCode = "PH" };
                yield return new City { Id = 1166000, Name = "Sargodha", Latitude = 32.08361m, Longitude = 72.67111m, CountryCode = "PK" };
                yield return new City { Id = 1166993, Name = "Rawalpindi", Latitude = 33.6007m, Longitude = 73.0679m, CountryCode = "PK" };
                yield return new City { Id = 1167528, Name = "Quetta", Latitude = 30.18414m, Longitude = 67.00141m, CountryCode = "PK" };
                yield return new City { Id = 1168197, Name = "Peshawar", Latitude = 34.008m, Longitude = 71.57849m, CountryCode = "PK" };
                yield return new City { Id = 1169607, Name = "Muzaffarābād", Latitude = 34.37002m, Longitude = 73.47082m, CountryCode = "PK" };
                yield return new City { Id = 1169825, Name = "Multān", Latitude = 30.19679m, Longitude = 71.47824m, CountryCode = "PK" };
                yield return new City { Id = 1172451, Name = "Lahore", Latitude = 31.54972m, Longitude = 74.34361m, CountryCode = "PK" };
                yield return new City { Id = 1173055, Name = "Kotli", Latitude = 33.51836m, Longitude = 73.9022m, CountryCode = "PK" };
                yield return new City { Id = 1174872, Name = "Karachi", Latitude = 24.9056m, Longitude = 67.0822m, CountryCode = "PK" };
                yield return new City { Id = 1176615, Name = "Islamabad", Latitude = 33.72148m, Longitude = 73.04329m, CountryCode = "PK" };
                yield return new City { Id = 1176734, Name = "Hyderabad", Latitude = 25.39242m, Longitude = 68.37366m, CountryCode = "PK" };
                yield return new City { Id = 1177662, Name = "Gujrānwāla", Latitude = 32.15567m, Longitude = 74.18705m, CountryCode = "PK" };
                yield return new City { Id = 1179400, Name = "Faisalābād", Latitude = 31.41667m, Longitude = 73.08333m, CountryCode = "PK" };
                yield return new City { Id = 1183105, Name = "Battagram", Latitude = 34.67719m, Longitude = 73.02329m, CountryCode = "PK" };
                yield return new City { Id = 1183880, Name = "Bahāwalpur", Latitude = 29.4m, Longitude = 71.68333m, CountryCode = "PK" };
                yield return new City { Id = 756135, Name = "Warsaw", Latitude = 52.22977m, Longitude = 21.01178m, CountryCode = "PL" };
                yield return new City { Id = 3081368, Name = "Wrocław", Latitude = 51.1m, Longitude = 17.03333m, CountryCode = "PL" };
                yield return new City { Id = 3088171, Name = "Poznań", Latitude = 52.40692m, Longitude = 16.92993m, CountryCode = "PL" };
                yield return new City { Id = 3093133, Name = "Łódź", Latitude = 51.75m, Longitude = 19.46667m, CountryCode = "PL" };
                yield return new City { Id = 3094802, Name = "Kraków", Latitude = 50.06143m, Longitude = 19.93658m, CountryCode = "PL" };
                yield return new City { Id = 3424934, Name = "Saint-Pierre", Latitude = 46.77914m, Longitude = -56.1773m, CountryCode = "PM" };
                yield return new City { Id = 4030723, Name = "Adamstown", Latitude = -25.06597m, Longitude = -130.10147m, CountryCode = "PN" };
                yield return new City { Id = 4568127, Name = "San Juan", Latitude = 18.46633m, Longitude = -66.10572m, CountryCode = "PR" };
                yield return new City { Id = 282239, Name = "Ramallah", Latitude = 31.89964m, Longitude = 35.20422m, CountryCode = "PS" };
                yield return new City { Id = 2267057, Name = "Lisbon", Latitude = 38.71667m, Longitude = -9.13333m, CountryCode = "PT" };
                yield return new City { Id = 3439389, Name = "Asunción", Latitude = -25.30066m, Longitude = -57.63591m, CountryCode = "PY" };
                yield return new City { Id = 290030, Name = "Doha", Latitude = 25.27932m, Longitude = 51.52245m, CountryCode = "QA" };
                yield return new City { Id = 935214, Name = "Saint-Pierre", Latitude = -21.3393m, Longitude = 55.47811m, CountryCode = "RE" };
                yield return new City { Id = 935264, Name = "Saint-Denis", Latitude = -20.88231m, Longitude = 55.4504m, CountryCode = "RE" };
                yield return new City { Id = 683506, Name = "Bucharest", Latitude = 44.43225m, Longitude = 26.10626m, CountryCode = "RO" };
                yield return new City { Id = 792680, Name = "Belgrade", Latitude = 44.80401m, Longitude = 20.46513m, CountryCode = "RS" };
                yield return new City { Id = 468902, Name = "Yaroslavl", Latitude = 57.62987m, Longitude = 39.87368m, CountryCode = "RU" };
                yield return new City { Id = 472045, Name = "Voronezh", Latitude = 51.67204m, Longitude = 39.1843m, CountryCode = "RU" };
                yield return new City { Id = 472757, Name = "Volgograd", Latitude = 48.71939m, Longitude = 44.50183m, CountryCode = "RU" };
                yield return new City { Id = 479123, Name = "Ulyanovsk", Latitude = 54.32824m, Longitude = 48.38657m, CountryCode = "RU" };
                yield return new City { Id = 479561, Name = "Ufa", Latitude = 54.74306m, Longitude = 55.96779m, CountryCode = "RU" };
                yield return new City { Id = 482283, Name = "Tol’yatti", Latitude = 53.5303m, Longitude = 49.3461m, CountryCode = "RU" };
                yield return new City { Id = 498677, Name = "Saratov", Latitude = 51.54056m, Longitude = 46.00861m, CountryCode = "RU" };
                yield return new City { Id = 498817, Name = "Saint Petersburg", Latitude = 59.93863m, Longitude = 30.31413m, CountryCode = "RU" };
                yield return new City { Id = 499099, Name = "Samara", Latitude = 53.20007m, Longitude = 50.15m, CountryCode = "RU" };
                yield return new City { Id = 500096, Name = "Ryazan’", Latitude = 54.6269m, Longitude = 39.6916m, CountryCode = "RU" };
                yield return new City { Id = 501175, Name = "Rostov-na-Donu", Latitude = 47.23135m, Longitude = 39.72328m, CountryCode = "RU" };
                yield return new City { Id = 511196, Name = "Perm", Latitude = 58.01046m, Longitude = 56.25017m, CountryCode = "RU" };
                yield return new City { Id = 511565, Name = "Penza", Latitude = 53.20066m, Longitude = 45.00464m, CountryCode = "RU" };
                yield return new City { Id = 515003, Name = "Orenburg", Latitude = 51.7727m, Longitude = 55.0988m, CountryCode = "RU" };
                yield return new City { Id = 520555, Name = "Nizhniy Novgorod", Latitude = 56.32867m, Longitude = 44.00205m, CountryCode = "RU" };
                yield return new City { Id = 523750, Name = "Naberezhnyye Chelny", Latitude = 55.72545m, Longitude = 52.41122m, CountryCode = "RU" };
                yield return new City { Id = 524901, Name = "Moscow", Latitude = 55.75222m, Longitude = 37.61556m, CountryCode = "RU" };
                yield return new City { Id = 535121, Name = "Lipetsk", Latitude = 52.60311m, Longitude = 39.57076m, CountryCode = "RU" };
                yield return new City { Id = 542420, Name = "Krasnodar", Latitude = 45.04484m, Longitude = 38.97603m, CountryCode = "RU" };
                yield return new City { Id = 551487, Name = "Kazan", Latitude = 55.78874m, Longitude = 49.12214m, CountryCode = "RU" };
                yield return new City { Id = 554840, Name = "Izhevsk", Latitude = 56.84976m, Longitude = 53.20448m, CountryCode = "RU" };
                yield return new City { Id = 580497, Name = "Astrakhan", Latitude = 46.34968m, Longitude = 48.04076m, CountryCode = "RU" };
                yield return new City { Id = 1486209, Name = "Yekaterinburg", Latitude = 56.8519m, Longitude = 60.6122m, CountryCode = "RU" };
                yield return new City { Id = 1488754, Name = "Tyumen", Latitude = 57.15222m, Longitude = 65.52722m, CountryCode = "RU" };
                yield return new City { Id = 1496153, Name = "Omsk", Latitude = 54.99244m, Longitude = 73.36859m, CountryCode = "RU" };
                yield return new City { Id = 1496747, Name = "Novosibirsk", Latitude = 55.0415m, Longitude = 82.9346m, CountryCode = "RU" };
                yield return new City { Id = 1496990, Name = "Novokuznetsk", Latitude = 53.7557m, Longitude = 87.1099m, CountryCode = "RU" };
                yield return new City { Id = 1502026, Name = "Krasnoyarsk", Latitude = 56.01839m, Longitude = 92.86717m, CountryCode = "RU" };
                yield return new City { Id = 1508291, Name = "Chelyabinsk", Latitude = 55.15402m, Longitude = 61.42915m, CountryCode = "RU" };
                yield return new City { Id = 1510853, Name = "Barnaul", Latitude = 53.36056m, Longitude = 83.76361m, CountryCode = "RU" };
                yield return new City { Id = 2013348, Name = "Vladivostok", Latitude = 43.10562m, Longitude = 131.87353m, CountryCode = "RU" };
                yield return new City { Id = 2022890, Name = "Khabarovsk", Latitude = 48.48271m, Longitude = 135.08379m, CountryCode = "RU" };
                yield return new City { Id = 2023469, Name = "Irkutsk", Latitude = 52.29778m, Longitude = 104.29639m, CountryCode = "RU" };
                yield return new City { Id = 2056752, Name = "Khabarovsk Vtoroy", Latitude = 48.43776m, Longitude = 135.13329m, CountryCode = "RU" };
                yield return new City { Id = 8504951, Name = "Kalininskiy", Latitude = 59.99675m, Longitude = 30.3899m, CountryCode = "RU" };
                yield return new City { Id = 202061, Name = "Kigali", Latitude = -1.94995m, Longitude = 30.05885m, CountryCode = "RW" };
                yield return new City { Id = 101760, Name = "Sulţānah", Latitude = 24.49258m, Longitude = 39.58572m, CountryCode = "SA" };
                yield return new City { Id = 104515, Name = "Mecca", Latitude = 21.42664m, Longitude = 39.82563m, CountryCode = "SA" };
                yield return new City { Id = 105343, Name = "Jeddah", Latitude = 21.54238m, Longitude = 39.19797m, CountryCode = "SA" };
                yield return new City { Id = 107968, Name = "Ta’if", Latitude = 21.27028m, Longitude = 40.41583m, CountryCode = "SA" };
                yield return new City { Id = 108410, Name = "Riyadh", Latitude = 24.68773m, Longitude = 46.72185m, CountryCode = "SA" };
                yield return new City { Id = 109223, Name = "Medina", Latitude = 24.46861m, Longitude = 39.61417m, CountryCode = "SA" };
                yield return new City { Id = 110336, Name = "Dammam", Latitude = 26.43442m, Longitude = 50.10326m, CountryCode = "SA" };
                yield return new City { Id = 2108502, Name = "Honiara", Latitude = -9.43333m, Longitude = 159.95m, CountryCode = "SB" };
                yield return new City { Id = 241131, Name = "Victoria", Latitude = -4.61667m, Longitude = 55.45m, CountryCode = "SC" };
                yield return new City { Id = 365137, Name = "Omdurman", Latitude = 15.64453m, Longitude = 32.47773m, CountryCode = "SD" };
                yield return new City { Id = 379252, Name = "Khartoum", Latitude = 15.55177m, Longitude = 32.53241m, CountryCode = "SD" };
                yield return new City { Id = 2673730, Name = "Stockholm", Latitude = 59.33258m, Longitude = 18.0649m, CountryCode = "SE" };
                yield return new City { Id = 2711537, Name = "Göteborg", Latitude = 57.70716m, Longitude = 11.96679m, CountryCode = "SE" };
                yield return new City { Id = 1880252, Name = "Singapore", Latitude = 1.28967m, Longitude = 103.85007m, CountryCode = "SG" };
                yield return new City { Id = 3370903, Name = "Jamestown", Latitude = -15.93872m, Longitude = -5.71675m, CountryCode = "SH" };
                yield return new City { Id = 3196359, Name = "Ljubljana", Latitude = 46.05108m, Longitude = 14.50513m, CountryCode = "SI" };
                yield return new City { Id = 2729907, Name = "Longyearbyen", Latitude = 78.22334m, Longitude = 15.64689m, CountryCode = "SJ" };
                yield return new City { Id = 3060972, Name = "Bratislava", Latitude = 48.14816m, Longitude = 17.10674m, CountryCode = "SK" };
                yield return new City { Id = 2409306, Name = "Freetown", Latitude = 8.484m, Longitude = -13.22994m, CountryCode = "SL" };
                yield return new City { Id = 3168070, Name = "City of San Marino", Latitude = 43.93667m, Longitude = 12.44639m, CountryCode = "SM" };
                yield return new City { Id = 2244322, Name = "Touba", Latitude = 14.85m, Longitude = -15.88333m, CountryCode = "SN" };
                yield return new City { Id = 2246678, Name = "Pikine", Latitude = 14.76457m, Longitude = -17.39071m, CountryCode = "SN" };
                yield return new City { Id = 2253354, Name = "Dakar", Latitude = 14.6937m, Longitude = -17.44406m, CountryCode = "SN" };
                yield return new City { Id = 53654, Name = "Mogadishu", Latitude = 2.03711m, Longitude = 45.34375m, CountryCode = "SO" };
                yield return new City { Id = 3383330, Name = "Paramaribo", Latitude = 5.86638m, Longitude = -55.16682m, CountryCode = "SR" };
                yield return new City { Id = 373303, Name = "Juba", Latitude = 4.85165m, Longitude = 31.58247m, CountryCode = "SS" };
                yield return new City { Id = 2410763, Name = "São Tomé", Latitude = 0.33654m, Longitude = 6.72732m, CountryCode = "ST" };
                yield return new City { Id = 3583361, Name = "San Salvador", Latitude = 13.68935m, Longitude = -89.18718m, CountryCode = "SV" };
                yield return new City { Id = 3513392, Name = "Philipsburg", Latitude = 18.026m, Longitude = -63.04582m, CountryCode = "SX" };
                yield return new City { Id = 169577, Name = "Homs", Latitude = 34.72682m, Longitude = 36.72339m, CountryCode = "SY" };
                yield return new City { Id = 170063, Name = "Aleppo", Latitude = 36.20124m, Longitude = 37.16117m, CountryCode = "SY" };
                yield return new City { Id = 170654, Name = "Damascus", Latitude = 33.5102m, Longitude = 36.29128m, CountryCode = "SY" };
                yield return new City { Id = 935048, Name = "Lobamba", Latitude = -26.46667m, Longitude = 31.2m, CountryCode = "SZ" };
                yield return new City { Id = 3576994, Name = "Cockburn Town", Latitude = 21.46122m, Longitude = -71.14188m, CountryCode = "TC" };
                yield return new City { Id = 2427123, Name = "N'Djamena", Latitude = 12.10672m, Longitude = 15.0444m, CountryCode = "TD" };
                yield return new City { Id = 1546102, Name = "Port-aux-Français", Latitude = -49.35m, Longitude = 70.21667m, CountryCode = "TF" };
                yield return new City { Id = 2365267, Name = "Lomé", Latitude = 6.13748m, Longitude = 1.21227m, CountryCode = "TG" };
                yield return new City { Id = 1609350, Name = "Bangkok", Latitude = 13.75398m, Longitude = 100.50144m, CountryCode = "TH" };
                yield return new City { Id = 1221874, Name = "Dushanbe", Latitude = 38.53575m, Longitude = 68.77905m, CountryCode = "TJ" };
                yield return new City { Id = 1645457, Name = "Dili", Latitude = -8.55861m, Longitude = 125.57361m, CountryCode = "TL" };
                yield return new City { Id = 162183, Name = "Ashgabat", Latitude = 37.95m, Longitude = 58.38333m, CountryCode = "TM" };
                yield return new City { Id = 2464470, Name = "Tunis", Latitude = 36.81897m, Longitude = 10.16579m, CountryCode = "TN" };
                yield return new City { Id = 4032402, Name = "Nuku'alofa", Latitude = -21.13938m, Longitude = -175.2018m, CountryCode = "TO" };
                yield return new City { Id = 304531, Name = "Mercin", Latitude = 36.79526m, Longitude = 34.61792m, CountryCode = "TR" };
                yield return new City { Id = 306571, Name = "Konya", Latitude = 37.87135m, Longitude = 32.48464m, CountryCode = "TR" };
                yield return new City { Id = 308464, Name = "Kayseri", Latitude = 38.73222m, Longitude = 35.48528m, CountryCode = "TR" };
                yield return new City { Id = 311046, Name = "İzmir", Latitude = 38.41273m, Longitude = 27.13838m, CountryCode = "TR" };
                yield return new City { Id = 314830, Name = "Gaziantep", Latitude = 37.05944m, Longitude = 37.3825m, CountryCode = "TR" };
                yield return new City { Id = 315202, Name = "Eskişehir", Latitude = 39.77667m, Longitude = 30.52056m, CountryCode = "TR" };
                yield return new City { Id = 316541, Name = "Diyarbakır", Latitude = 37.91363m, Longitude = 40.21721m, CountryCode = "TR" };
                yield return new City { Id = 323777, Name = "Antalya", Latitude = 36.90812m, Longitude = 30.69556m, CountryCode = "TR" };
                yield return new City { Id = 323786, Name = "Ankara", Latitude = 39.91987m, Longitude = 32.85427m, CountryCode = "TR" };
                yield return new City { Id = 325363, Name = "Adana", Latitude = 37.00167m, Longitude = 35.32889m, CountryCode = "TR" };
                yield return new City { Id = 738329, Name = "Üsküdar", Latitude = 41.02252m, Longitude = 29.02369m, CountryCode = "TR" };
                yield return new City { Id = 738377, Name = "Umraniye", Latitude = 41.01643m, Longitude = 29.12476m, CountryCode = "TR" };
                yield return new City { Id = 745044, Name = "İstanbul", Latitude = 41.01384m, Longitude = 28.94966m, CountryCode = "TR" };
                yield return new City { Id = 747340, Name = "Esenler", Latitude = 41.0435m, Longitude = 28.87619m, CountryCode = "TR" };
                yield return new City { Id = 750269, Name = "Bursa", Latitude = 40.19559m, Longitude = 29.06013m, CountryCode = "TR" };
                yield return new City { Id = 751324, Name = "Bağcılar", Latitude = 41.03903m, Longitude = 28.85671m, CountryCode = "TR" };
                yield return new City { Id = 6955677, Name = "Çankaya", Latitude = 39.9179m, Longitude = 32.86268m, CountryCode = "TR" };
                yield return new City { Id = 7627067, Name = "Bahçelievler", Latitude = 41.00231m, Longitude = 28.8598m, CountryCode = "TR" };
                yield return new City { Id = 3573703, Name = "Port Louis", Latitude = 11.18333m, Longitude = -60.73333m, CountryCode = "TT" };
                yield return new City { Id = 3573890, Name = "Port of Spain", Latitude = 10.66668m, Longitude = -61.51889m, CountryCode = "TT" };
                yield return new City { Id = 2110394, Name = "Funafuti", Latitude = -8.52425m, Longitude = 179.19417m, CountryCode = "TV" };
                yield return new City { Id = 1668341, Name = "Taipei", Latitude = 25.04776m, Longitude = 121.53185m, CountryCode = "TW" };
                yield return new City { Id = 1668355, Name = "Tainan", Latitude = 22.99083m, Longitude = 120.21333m, CountryCode = "TW" };
                yield return new City { Id = 1668399, Name = "Taichung", Latitude = 24.1469m, Longitude = 120.6839m, CountryCode = "TW" };
                yield return new City { Id = 1670029, Name = "Banqiao", Latitude = 25.01427m, Longitude = 121.46719m, CountryCode = "TW" };
                yield return new City { Id = 1673820, Name = "Kaohsiung", Latitude = 22.61626m, Longitude = 120.31333m, CountryCode = "TW" };
                yield return new City { Id = 160196, Name = "Dodoma", Latitude = -6.17221m, Longitude = 35.73947m, CountryCode = "TZ" };
                yield return new City { Id = 160263, Name = "Dar es Salaam", Latitude = -6.82349m, Longitude = 39.26951m, CountryCode = "TZ" };
                yield return new City { Id = 687700, Name = "Zaporizhia", Latitude = 47.82289m, Longitude = 35.19031m, CountryCode = "UA" };
                yield return new City { Id = 698740, Name = "Odessa", Latitude = 46.47747m, Longitude = 30.73262m, CountryCode = "UA" };
                yield return new City { Id = 700569, Name = "Mykolayiv", Latitude = 46.96591m, Longitude = 31.9974m, CountryCode = "UA" };
                yield return new City { Id = 702550, Name = "Lviv", Latitude = 49.83826m, Longitude = 24.02324m, CountryCode = "UA" };
                yield return new City { Id = 703448, Name = "Kiev", Latitude = 50.45466m, Longitude = 30.5238m, CountryCode = "UA" };
                yield return new City { Id = 703845, Name = "Kryvyi Rih", Latitude = 47.90966m, Longitude = 33.38044m, CountryCode = "UA" };
                yield return new City { Id = 706483, Name = "Kharkiv", Latitude = 49.98081m, Longitude = 36.25272m, CountryCode = "UA" };
                yield return new City { Id = 709717, Name = "Donetsk", Latitude = 48.023m, Longitude = 37.80224m, CountryCode = "UA" };
                yield return new City { Id = 709930, Name = "Dnipro", Latitude = 48.4593m, Longitude = 35.03865m, CountryCode = "UA" };
                yield return new City { Id = 232422, Name = "Kampala", Latitude = 0.31628m, Longitude = 32.58219m, CountryCode = "UG" };
                yield return new City { Id = 4140963, Name = "Washington, D.C.", Latitude = 38.89511m, Longitude = -77.03637m, CountryCode = "US" };
                yield return new City { Id = 4160021, Name = "Jacksonville", Latitude = 30.33218m, Longitude = -81.65565m, CountryCode = "US" };
                yield return new City { Id = 4167694, Name = "Panama City", Latitude = 30.15946m, Longitude = -85.65983m, CountryCode = "US" };
                yield return new City { Id = 4177703, Name = "Wellington", Latitude = 26.65868m, Longitude = -80.24144m, CountryCode = "US" };
                yield return new City { Id = 4259418, Name = "Indianapolis", Latitude = 39.76838m, Longitude = -86.15804m, CountryCode = "US" };
                yield return new City { Id = 4292686, Name = "Georgetown", Latitude = 38.2098m, Longitude = -84.55883m, CountryCode = "US" };
                yield return new City { Id = 4347778, Name = "Baltimore", Latitude = 39.29038m, Longitude = -76.61219m, CountryCode = "US" };
                yield return new City { Id = 4460243, Name = "Charlotte", Latitude = 35.22709m, Longitude = -80.84313m, CountryCode = "US" };
                yield return new City { Id = 4509177, Name = "Columbus", Latitude = 39.96118m, Longitude = -82.99879m, CountryCode = "US" };
                yield return new City { Id = 4544349, Name = "Oklahoma City", Latitude = 35.46756m, Longitude = -97.51643m, CountryCode = "US" };
                yield return new City { Id = 4560349, Name = "Philadelphia", Latitude = 39.95233m, Longitude = -75.16379m, CountryCode = "US" };
                yield return new City { Id = 4641239, Name = "Memphis", Latitude = 35.14953m, Longitude = -90.04898m, CountryCode = "US" };
                yield return new City { Id = 4644585, Name = "Nashville", Latitude = 36.16589m, Longitude = -86.78444m, CountryCode = "US" };
                yield return new City { Id = 4645421, Name = "New South Memphis", Latitude = 35.08676m, Longitude = -90.05676m, CountryCode = "US" };
                yield return new City { Id = 4671654, Name = "Austin", Latitude = 30.26715m, Longitude = -97.74306m, CountryCode = "US" };
                yield return new City { Id = 4684888, Name = "Dallas", Latitude = 32.78306m, Longitude = -96.80667m, CountryCode = "US" };
                yield return new City { Id = 4691930, Name = "Fort Worth", Latitude = 32.72541m, Longitude = -97.32085m, CountryCode = "US" };
                yield return new City { Id = 4693342, Name = "Georgetown", Latitude = 30.63269m, Longitude = -97.67723m, CountryCode = "US" };
                yield return new City { Id = 4699066, Name = "Houston", Latitude = 29.76328m, Longitude = -95.36327m, CountryCode = "US" };
                yield return new City { Id = 4726206, Name = "San Antonio", Latitude = 29.42412m, Longitude = -98.49363m, CountryCode = "US" };
                yield return new City { Id = 4726440, Name = "San Juan", Latitude = 26.18924m, Longitude = -98.15529m, CountryCode = "US" };
                yield return new City { Id = 4739157, Name = "Victoria", Latitude = 28.80527m, Longitude = -97.0036m, CountryCode = "US" };
                yield return new City { Id = 4887398, Name = "Chicago", Latitude = 41.85003m, Longitude = -87.65005m, CountryCode = "US" };
                yield return new City { Id = 4930956, Name = "Boston", Latitude = 42.35843m, Longitude = -71.05977m, CountryCode = "US" };
                yield return new City { Id = 4951305, Name = "South Boston", Latitude = 42.33343m, Longitude = -71.04949m, CountryCode = "US" };
                yield return new City { Id = 4990729, Name = "Detroit", Latitude = 42.33143m, Longitude = -83.04575m, CountryCode = "US" };
                yield return new City { Id = 5041926, Name = "Plymouth", Latitude = 45.01052m, Longitude = -93.45551m, CountryCode = "US" };
                yield return new City { Id = 5059836, Name = "Jamestown", Latitude = 46.91054m, Longitude = -98.70844m, CountryCode = "US" };
                yield return new City { Id = 5110266, Name = "The Bronx", Latitude = 40.84985m, Longitude = -73.86641m, CountryCode = "US" };
                yield return new City { Id = 5110302, Name = "Brooklyn", Latitude = 40.6501m, Longitude = -73.94958m, CountryCode = "US" };
                yield return new City { Id = 5122534, Name = "Jamestown", Latitude = 42.097m, Longitude = -79.23533m, CountryCode = "US" };
                yield return new City { Id = 5125771, Name = "Manhattan", Latitude = 40.78343m, Longitude = -73.96625m, CountryCode = "US" };
                yield return new City { Id = 5128581, Name = "New York City", Latitude = 40.71427m, Longitude = -74.00597m, CountryCode = "US" };
                yield return new City { Id = 5133273, Name = "Borough of Queens", Latitude = 40.68149m, Longitude = -73.83652m, CountryCode = "US" };
                yield return new City { Id = 5263045, Name = "Milwaukee", Latitude = 43.0389m, Longitude = -87.90647m, CountryCode = "US" };
                yield return new City { Id = 5293083, Name = "Douglas", Latitude = 31.34455m, Longitude = -109.54534m, CountryCode = "US" };
                yield return new City { Id = 5308655, Name = "Phoenix", Latitude = 33.44838m, Longitude = -112.07404m, CountryCode = "US" };
                yield return new City { Id = 5318313, Name = "Tucson", Latitude = 32.22174m, Longitude = -110.92648m, CountryCode = "US" };
                yield return new City { Id = 5368361, Name = "Los Angeles", Latitude = 34.05223m, Longitude = -118.24368m, CountryCode = "US" };
                yield return new City { Id = 5391811, Name = "San Diego", Latitude = 32.71533m, Longitude = -117.15726m, CountryCode = "US" };
                yield return new City { Id = 5391959, Name = "San Francisco", Latitude = 37.77493m, Longitude = -122.41942m, CountryCode = "US" };
                yield return new City { Id = 5392171, Name = "San Jose", Latitude = 37.33939m, Longitude = -121.89496m, CountryCode = "US" };
                yield return new City { Id = 5419384, Name = "Denver", Latitude = 39.73915m, Longitude = -104.9847m, CountryCode = "US" };
                yield return new City { Id = 5454711, Name = "Albuquerque", Latitude = 35.08449m, Longitude = -106.65114m, CountryCode = "US" };
                yield return new City { Id = 5506956, Name = "Las Vegas", Latitude = 36.17497m, Longitude = -115.13722m, CountryCode = "US" };
                yield return new City { Id = 5520993, Name = "El Paso", Latitude = 31.75872m, Longitude = -106.48693m, CountryCode = "US" };
                yield return new City { Id = 5746545, Name = "Portland", Latitude = 45.52345m, Longitude = -122.67621m, CountryCode = "US" };
                yield return new City { Id = 5809844, Name = "Seattle", Latitude = 47.60621m, Longitude = -122.33207m, CountryCode = "US" };
                yield return new City { Id = 3440639, Name = "San José", Latitude = -34.3375m, Longitude = -56.71361m, CountryCode = "UY" };
                yield return new City { Id = 3441575, Name = "Montevideo", Latitude = -34.90328m, Longitude = -56.18816m, CountryCode = "UY" };
                yield return new City { Id = 1512569, Name = "Tashkent", Latitude = 41.26465m, Longitude = 69.21627m, CountryCode = "UZ" };
                yield return new City { Id = 6691831, Name = "Vatican City", Latitude = 41.90268m, Longitude = 12.45414m, CountryCode = "VA" };
                yield return new City { Id = 3577887, Name = "Kingstown", Latitude = 13.15527m, Longitude = -61.22742m, CountryCode = "VC" };
                yield return new City { Id = 3625549, Name = "Valencia", Latitude = 10.16202m, Longitude = -68.00765m, CountryCode = "VE" };
                yield return new City { Id = 3632998, Name = "Maracay", Latitude = 10.23535m, Longitude = -67.59113m, CountryCode = "VE" };
                yield return new City { Id = 3633009, Name = "Maracaibo", Latitude = 10.66663m, Longitude = -71.61245m, CountryCode = "VE" };
                yield return new City { Id = 3645528, Name = "Ciudad Guayana", Latitude = 8.35122m, Longitude = -62.64102m, CountryCode = "VE" };
                yield return new City { Id = 3646738, Name = "Caracas", Latitude = 10.48801m, Longitude = -66.87919m, CountryCode = "VE" };
                yield return new City { Id = 3648522, Name = "Barquisimeto", Latitude = 10.0647m, Longitude = -69.35703m, CountryCode = "VE" };
                yield return new City { Id = 3577430, Name = "Road Town", Latitude = 18.42693m, Longitude = -64.62079m, CountryCode = "VG" };
                yield return new City { Id = 4795467, Name = "Charlotte Amalie", Latitude = 18.3419m, Longitude = -64.9307m, CountryCode = "VI" };
                yield return new City { Id = 1566083, Name = "Ho Chi Minh City", Latitude = 10.82302m, Longitude = 106.62965m, CountryCode = "VN" };
                yield return new City { Id = 1581130, Name = "Hanoi", Latitude = 21.0245m, Longitude = 105.84117m, CountryCode = "VN" };
                yield return new City { Id = 1581298, Name = "Haiphong", Latitude = 20.86481m, Longitude = 106.68345m, CountryCode = "VN" };
                yield return new City { Id = 1583992, Name = "Da Nang", Latitude = 16.06778m, Longitude = 108.22083m, CountryCode = "VN" };
                yield return new City { Id = 2135171, Name = "Port Vila", Latitude = -17.73381m, Longitude = 168.32188m, CountryCode = "VU" };
                yield return new City { Id = 4034821, Name = "Mata-Utu", Latitude = -13.28163m, Longitude = -176.17453m, CountryCode = "WF" };
                yield return new City { Id = 4035413, Name = "Apia", Latitude = -13.83333m, Longitude = -171.76666m, CountryCode = "WS" };
                yield return new City { Id = 786714, Name = "Pristina", Latitude = 42.67272m, Longitude = 21.16688m, CountryCode = "XK" };
                yield return new City { Id = 70225, Name = "Ta‘izz", Latitude = 13.57952m, Longitude = 44.02091m, CountryCode = "YE" };
                yield return new City { Id = 71137, Name = "Sanaa", Latitude = 15.35472m, Longitude = 44.20667m, CountryCode = "YE" };
                yield return new City { Id = 79415, Name = "Al Ḩudaydah", Latitude = 14.79781m, Longitude = 42.95452m, CountryCode = "YE" };
                yield return new City { Id = 415189, Name = "Aden", Latitude = 12.77944m, Longitude = 45.03667m, CountryCode = "YE" };
                yield return new City { Id = 921815, Name = "Mamoudzou", Latitude = -12.78234m, Longitude = 45.22878m, CountryCode = "YT" };
                yield return new City { Id = 949880, Name = "Tembisa", Latitude = -25.99636m, Longitude = 28.2268m, CountryCode = "ZA" };
                yield return new City { Id = 953781, Name = "Soweto", Latitude = -26.26781m, Longitude = 27.85849m, CountryCode = "ZA" };
                yield return new City { Id = 964137, Name = "Pretoria", Latitude = -25.74486m, Longitude = 28.18783m, CountryCode = "ZA" };
                yield return new City { Id = 964420, Name = "Port Elizabeth", Latitude = -33.91799m, Longitude = 25.57007m, CountryCode = "ZA" };
                yield return new City { Id = 965301, Name = "Pietermaritzburg", Latitude = -29.61679m, Longitude = 30.39278m, CountryCode = "ZA" };
                yield return new City { Id = 993800, Name = "Johannesburg", Latitude = -26.20227m, Longitude = 28.04363m, CountryCode = "ZA" };
                yield return new City { Id = 1007311, Name = "Durban", Latitude = -29.8579m, Longitude = 31.0292m, CountryCode = "ZA" };
                yield return new City { Id = 1020098, Name = "Benoni", Latitude = -26.18848m, Longitude = 28.32078m, CountryCode = "ZA" };
                yield return new City { Id = 3359510, Name = "Wellington", Latitude = -33.63981m, Longitude = 19.0112m, CountryCode = "ZA" };
                yield return new City { Id = 3369157, Name = "Cape Town", Latitude = -33.92584m, Longitude = 18.42322m, CountryCode = "ZA" };
                yield return new City { Id = 909137, Name = "Lusaka", Latitude = -15.40669m, Longitude = 28.28713m, CountryCode = "ZM" };
                yield return new City { Id = 890299, Name = "Harare", Latitude = -17.82772m, Longitude = 31.05337m, CountryCode = "ZW" };
                yield return new City { Id = 894701, Name = "Bulawayo", Latitude = -20.15m, Longitude = 28.58333m, CountryCode = "ZW" };
            }
        }
    }
}
