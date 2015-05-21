namespace EA.Weee.Requests.Notification
{
    using System;
    using Prsd.Core.Mediator;

    public class GetExporterByNotificationId : IRequest<ExporterData>
    {
        public Guid NotificationId { get; set; }

        public GetExporterByNotificationId(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
