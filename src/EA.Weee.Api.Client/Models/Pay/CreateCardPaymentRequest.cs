namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CreateCardPaymentRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("language")]
        public string Language => "en";

        //[JsonPropertyName("moto")]
        //public bool Moto { get; set; }

        //[JsonPropertyName("metadata")]
        //public Dictionary<string, string> Metadata { get; set; }
    }
}
