namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;

    public class RefundSummary
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount_available")]
        public int AmountAvailable { get; set; }

        [JsonProperty("amount_submitted")]
        public int AmountSubmitted { get; set; }
    }
}
