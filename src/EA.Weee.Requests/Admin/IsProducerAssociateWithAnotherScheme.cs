namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class IsProducerAssociateWithAnotherScheme : IRequest<bool>
    {
        public string RegistrationNumber { get; set; }

        public int ComplianceYear { get; set; }

        public IsProducerAssociateWithAnotherScheme(string registrationNumber, int complianceYear)
        {
            RegistrationNumber = registrationNumber;
            ComplianceYear = complianceYear;
        }
    }
}
