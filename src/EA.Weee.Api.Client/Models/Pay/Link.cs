namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Text.Json.Serialization;

    public class Link
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }
    }
}
