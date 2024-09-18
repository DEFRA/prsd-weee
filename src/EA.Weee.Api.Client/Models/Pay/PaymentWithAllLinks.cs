namespace EA.Weee.Api.Client.Models.Pay
{
    using System;
    using System.Text.Json.Serialization;

    public class PaymentWithAllLinks
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("state")]
        public PaymentState State { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("payment_provider")]
        public string PaymentProvider { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("refund_summary")]
        public RefundSummary RefundSummary { get; set; }

        [JsonPropertyName("settlement_summary")]
        public SettlementSummary SettlementSummary { get; set; }

        [JsonPropertyName("_links")]
        public PaymentLinks Links { get; set; }
    }
}
