namespace EA.Weee.Requests.Facilities
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class NotificationTypeByNotificationId : IRequest<NotificationType>
    {
        public Guid NotificationId { get; private set; }

        public NotificationTypeByNotificationId(Guid notificationId)
        {
            this.NotificationId = notificationId;
        }
    }
}
