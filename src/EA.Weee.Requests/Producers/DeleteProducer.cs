namespace EA.Weee.Requests.Producers
{
    using System;
    using Prsd.Core.Mediator;

    public class DeleteProducer : IRequest<bool>
    {
        public DeleteProducer(Guid producerId, Guid notificationId)
        {
            ProducerId = producerId;
            NotificationId = notificationId;
        }

        public Guid ProducerId { get; private set; }

        public Guid NotificationId { get; private set; }
    }
}