namespace EA.Weee.Web.Controllers
{
    using System.Text.Json.Serialization;

    public class RegisteredOffice
    {
        public string BuildingNumber { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public Country Country { get; set; }
        public string Postcode { get; set; }
        public bool IsUkAddress { get; set; }
        public string Checksum { get; set; }
    }
}