namespace EA.Weee.Api.Client.Models
{
    using EA.Weee.Core.Organisations;
    using Newtonsoft.Json;

    public class CompaniesHouseApiModel
    {
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("company_number")]
        public string CompanyNumber { get; set; }

        [JsonProperty("company_status")]
        public string CompanyStatus { get; set; }

        [JsonProperty("type")]
        public string CompanyType { get; set; }

        [JsonProperty("registered_office_address")]
        public ExternalAddressData ExternalAddressData { get; set; }

        // public CompanyProfile CompanyProfile { get; set; }
    }
}
