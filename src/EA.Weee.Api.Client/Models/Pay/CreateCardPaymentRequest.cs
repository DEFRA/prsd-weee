namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class CreateCardPaymentRequest
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("moto")]
        public bool Moto { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}
