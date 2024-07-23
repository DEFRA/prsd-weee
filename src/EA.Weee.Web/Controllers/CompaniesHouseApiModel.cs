namespace EA.Weee.Web.Controllers
{
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
        public RegisteredOfficeAddress RegisteredOfficeAddress { get; set; }

        public CompanyProfile CompanyProfile { get; set; }
    }
}