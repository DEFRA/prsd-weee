namespace EA.Weee.Requests.Admin
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;

    public class GetProducerDetails : IRequest<ProducerDetails>
    {
        public string RegistrationNumber { get; set; }
    }
}
