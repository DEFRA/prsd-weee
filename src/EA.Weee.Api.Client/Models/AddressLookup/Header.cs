namespace EA.Weee.Api.Client.Models.AddressLookup
{
    using System.Text.Json.Serialization;

    public class Header
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("offset")]
        public string Offset { get; set; }

        [JsonPropertyName("totalResults")]
        public string TotalResults { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("dataset")]
        public string Dataset { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("maximumResults")]
        public string MaximumResults { get; set; }

        [JsonPropertyName("matchingTotalResults")]
        public string MatchingTotalResults { get; set; }
    }
}
