namespace EA.Weee.Api.Client.Models.Pay
{
    using Newtonsoft.Json;

    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }
}
