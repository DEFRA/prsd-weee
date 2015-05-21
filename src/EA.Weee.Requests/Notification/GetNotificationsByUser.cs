namespace EA.Weee.Requests.Notification
{
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class GetNotificationsByUser : IRequest<IList<NotificationApplicationSummaryData>>
    {
    }
}