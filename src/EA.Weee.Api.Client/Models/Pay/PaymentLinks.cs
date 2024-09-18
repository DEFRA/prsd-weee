namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;

    public class PaymentLinks
    {
        [JsonProperty("self")]
        public Link Self { get; set; }

        [JsonProperty("next_url")]
        public Link NextUrl { get; set; }

        [JsonProperty("next_url_post")]
        public Link NextUrlPost { get; set; }

        [JsonProperty("events")]
        public Link Events { get; set; }

        [JsonProperty("refunds")]
        public Link Refunds { get; set; }

        [JsonProperty("cancel")]
        public Link Cancel { get; set; }
    }
}
