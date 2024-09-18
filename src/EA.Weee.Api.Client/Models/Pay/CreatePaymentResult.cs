namespace EA.Weee.Api.Client.Models.Pay
{
    using System;
    using System.Text.Json.Serialization;

    public class CreatePaymentResult
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("state")]
        public PaymentState State { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("_links")]
        public PaymentLinks Links { get; set; }
    }
}
