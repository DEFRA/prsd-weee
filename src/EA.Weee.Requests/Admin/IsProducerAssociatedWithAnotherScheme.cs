namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class IsProducerAssociatedWithAnotherScheme : IRequest<bool>
    {
        public string RegistrationNumber { get; set; }

        public int ComplianceYear { get; set; }

        public IsProducerAssociatedWithAnotherScheme(string registrationNumber, int complianceYear)
        {
            RegistrationNumber = registrationNumber;
            ComplianceYear = complianceYear;
        }
    }
}
