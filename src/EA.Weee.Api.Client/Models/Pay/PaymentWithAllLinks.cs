namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;
    using System;

    public class PaymentWithAllLinks
    {
        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("state")]
        public PaymentState State { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("payment_provider")]
        public string PaymentProvider { get; set; }

        [JsonProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("refund_summary")]
        public RefundSummary RefundSummary { get; set; }

        [JsonProperty("settlement_summary")]
        public SettlementSummary SettlementSummary { get; set; }

        [JsonProperty("_links")]
        public PaymentLinks Links { get; set; }
    }
}
