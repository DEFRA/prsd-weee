namespace EA.Weee.Api.Client.Models
{
    public class RegisteredOffice
    {
        public string BuildingName { get; set; }
        public string BuildingNumber { get; set; }
        public string SubBuildingName { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public Country Country { get; set; }
        public string Postcode { get; set; }
    }
}
