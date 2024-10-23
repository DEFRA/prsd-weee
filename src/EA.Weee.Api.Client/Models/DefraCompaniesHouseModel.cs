namespace EA.Weee.Api.Client.Models
{
    public class DefraCompaniesHouseApiModel
    {
        public Organisation Organisation { get; set; }

        public bool InvalidReference { get; set; }

        public bool Error { get; set; }

        public bool HasError => InvalidReference || Error;
    }
}
