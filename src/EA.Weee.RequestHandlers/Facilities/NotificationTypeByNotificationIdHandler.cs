namespace EA.Weee.RequestHandlers.Facilities
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Facilities;
    using Requests.Shared;

    internal class NotificationTypeByNotificationIdHandler : IRequestHandler<NotificationTypeByNotificationId, NotificationType>
    {
        private readonly IwsContext context;

        public NotificationTypeByNotificationIdHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<NotificationType> HandleAsync(NotificationTypeByNotificationId message)
        {
            var notificationType = await context.NotificationApplications
                .Where(n => n.Id == message.NotificationId)
                .Select(n => n.NotificationType).SingleAsync();

            if (notificationType == Domain.Notification.NotificationType.Disposal)
            {
                return NotificationType.Disposal;
            }

            return NotificationType.Recovery;
        }
    }
}
