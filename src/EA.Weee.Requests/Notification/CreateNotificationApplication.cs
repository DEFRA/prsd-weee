namespace EA.Weee.Requests.Notification
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class CreateNotificationApplication : IRequest<Guid>
    {
        public NotificationType NotificationType { get; set; }

        public CompetentAuthority CompetentAuthority { get; set; }
    }
}