namespace EA.Weee.Api.Client.Models.Pay
{
    using EA.Weee.Core.DirectRegistrant;
    using System.Text.Json.Serialization;

    public class PaymentState
    {
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus Status { get; set; }

        [JsonPropertyName("finished")]
        public bool Finished { get; set; }

        public bool IsInFinalState()
        {
            return Status == PaymentStatus.Success
                   || Status == PaymentStatus.Failed
                   || Status == PaymentStatus.Cancelled
                   || Status == PaymentStatus.Error;
        }
    }
}
