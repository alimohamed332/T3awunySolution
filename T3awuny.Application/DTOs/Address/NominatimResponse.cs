using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Address
{
    public class NominatimResponse
    {
        [JsonPropertyName("address")]
        public NominatimAddress? Address { get; set; }
    }

    public class NominatimAddress
    {
        [JsonPropertyName("road")]
        public string? Road { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("postcode")]
        public string? PostCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}
