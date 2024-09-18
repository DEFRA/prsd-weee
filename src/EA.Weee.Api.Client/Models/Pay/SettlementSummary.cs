namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;
    using System;

    public class SettlementSummary
    {
        [JsonProperty("capture_submit_time")]
        public DateTime? CaptureSubmitTime { get; set; }

        [JsonProperty("captured_date")]
        public DateTime? CapturedDate { get; set; }
    }
}
