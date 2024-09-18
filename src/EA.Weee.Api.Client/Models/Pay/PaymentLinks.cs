namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Text.Json.Serialization;

    public class PaymentLinks
    {
        [JsonPropertyName("self")]
        public Link Self { get; set; }

        [JsonPropertyName("next_url")]
        public Link NextUrl { get; set; }

        [JsonPropertyName("next_url_post")]
        public Link NextUrlPost { get; set; }

        [JsonPropertyName("events")]
        public Link Events { get; set; }

        [JsonPropertyName("refunds")]
        public Link Refunds { get; set; }

        [JsonPropertyName("cancel")]
        public Link Cancel { get; set; }
    }
}
