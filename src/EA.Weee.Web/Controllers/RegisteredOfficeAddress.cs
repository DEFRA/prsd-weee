namespace EA.Weee.Web.Controllers
{
    using Newtonsoft.Json;

    public class RegisteredOfficeAddress
    {
        [JsonProperty("address_line_1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("address_line_2")]
        public string AddressLine2 { get; set; }
        public string CareOf { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("locality")]
        public string Locality { get; set; }
        public string PoBox { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        public string Premises { get; set; }
        public string Region { get; set; }
    }
}