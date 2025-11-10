namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Text.Json.Serialization;

    public class CreateCardPaymentRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

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
    }
}
