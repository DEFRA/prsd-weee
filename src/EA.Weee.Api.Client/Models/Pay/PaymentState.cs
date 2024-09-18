namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;

    public class PaymentState
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("finished")]
        public bool Finished { get; set; }
    }
}
