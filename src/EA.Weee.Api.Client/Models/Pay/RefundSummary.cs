namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Text.Json.Serialization;

    public class RefundSummary
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("amount_available")]
        public int AmountAvailable { get; set; }

        [JsonPropertyName("amount_submitted")]
        public int AmountSubmitted { get; set; }
    }
}
