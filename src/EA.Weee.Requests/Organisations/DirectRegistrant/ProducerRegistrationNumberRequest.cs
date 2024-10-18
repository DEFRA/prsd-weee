namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;

    public class ProducerRegistrationNumberRequest : IRequest<bool>
    {
        public string ProducerRegistrationNumber { get; private set; }

        public ProducerRegistrationNumberRequest(string producerRegistrationNumber)
        {
            ProducerRegistrationNumber = producerRegistrationNumber;
        }
    }
}
