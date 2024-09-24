namespace EA.Weee.Api.Client.Models.Pay
{
    using System;
    using System.Text.Json.Serialization;

    public class SettlementSummary
    {
        [JsonPropertyName("capture_submit_time")]
        public DateTime? CaptureSubmitTime { get; set; }

        [JsonPropertyName("captured_date")]
        public DateTime? CapturedDate { get; set; }
    }
}
