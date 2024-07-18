namespace EA.Weee.Web.Controllers
{
    using System.Text.Json.Serialization;
    public class Organisation
    {
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public OrganisationData OrganisationData { get; set; }
        public RegisteredOffice RegisteredOffice { get; set; }
        public string Checksum { get; set; }
    }
}