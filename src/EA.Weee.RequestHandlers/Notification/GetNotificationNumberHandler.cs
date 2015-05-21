namespace EA.Weee.RequestHandlers.Notification
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Notification;

    public class GetNotificationNumberHandler : IRequestHandler<GetNotificationNumber, string>
    {
        private readonly IwsContext context;
        private readonly IUserContext userContext;

        public GetNotificationNumberHandler(IwsContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<string> HandleAsync(GetNotificationNumber query)
        {
            var notification = await context
                .NotificationApplications
                .SingleAsync(n => n.Id == query.NotificationId
                && n.UserId == userContext.UserId);

            return notification.NotificationNumber;
        }
    }
}