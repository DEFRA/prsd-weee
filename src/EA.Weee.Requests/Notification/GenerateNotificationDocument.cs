namespace EA.Weee.Requests.Notification
{
    using System;
    using Prsd.Core.Mediator;

    public class GenerateNotificationDocument : IRequest<byte[]>
    {
        public GenerateNotificationDocument(Guid notificationId)
        {
            NotificationId = notificationId;
        }

        public Guid NotificationId { get; private set; }
    }
}