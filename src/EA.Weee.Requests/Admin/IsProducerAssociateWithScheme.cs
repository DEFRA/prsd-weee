namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;

    public class IsProducerAssociateWithScheme : IRequest<bool>
    {
        public string RegistrationNumber { get; set; }

        public IsProducerAssociateWithScheme(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
        }
    }
}
