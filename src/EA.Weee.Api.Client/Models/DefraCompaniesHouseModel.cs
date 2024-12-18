namespace EA.Weee.Api.Client.Models
{
    using System.Text.Json.Serialization;

    public class DefraCompaniesHouseApiModel
    {
        public Organisation Organisation { get; set; }

        [JsonIgnore]
        public bool InvalidReference { get; set; }

        [JsonIgnore]
        public bool Error { get; set; }

        [JsonIgnore]
        public bool HasError => InvalidReference || Error;
    }
}
