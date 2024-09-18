namespace EA.Weee.Api.Client.Models.Pay
{
    using System.Text.Json.Serialization;

    public class PaymentState
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("finished")]
        public bool Finished { get; set; }
    }
}
