namespace EA.Weee.Requests.Producers
{
    using System;
    using Prsd.Core.Mediator;

    public class AddProducerToNotification : IRequest<Guid>
    {
        public ProducerData ProducerData { get; private set; }

        public AddProducerToNotification(ProducerData producerData)
        {
            ProducerData = producerData;
        }
    }
}