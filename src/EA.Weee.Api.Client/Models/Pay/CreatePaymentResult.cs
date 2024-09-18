namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;
    using System;

    public class CreatePaymentResult
    {
        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("state")]
        public PaymentState State { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("_links")]
        public PaymentLinks Links { get; set; }
    }
}
