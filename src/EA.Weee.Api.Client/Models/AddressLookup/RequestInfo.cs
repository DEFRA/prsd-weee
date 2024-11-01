namespace EA.Weee.Api.Client.Models.AddressLookup
{
    using System;
    using System.Text.Json.Serialization;

    public class RequestInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("service")]
        public string Service { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("nodeID")]
        public string NodeId { get; set; }

        [JsonPropertyName("atomID")]
        public string AtomId { get; set; }
    }
}
