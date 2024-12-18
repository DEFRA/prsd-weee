namespace EA.Weee.Api.Client.Models
{
    public class Organisation
    {
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public OrganisationData OrganisationData { get; set; }
        public RegisteredOffice RegisteredOffice { get; set; }
        public string Checksum { get; set; }
    }
}
