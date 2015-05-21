namespace EA.Weee.RequestHandlers.Notification
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Notification;

    internal class GetNotificationsByUserHandler :
        IRequestHandler<GetNotificationsByUser, IList<NotificationApplicationSummaryData>>
    {
        private readonly IwsContext context;
        private readonly IUserContext userContext;

        public GetNotificationsByUserHandler(IwsContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<IList<NotificationApplicationSummaryData>> HandleAsync(GetNotificationsByUser query)
        {
            return
                await
                    context.NotificationApplications.Where(na => na.UserId == userContext.UserId)
                        .Select(n => new NotificationApplicationSummaryData
                        {
                            Id = n.Id,
                            NotificationNumber = n.NotificationNumber,
                            CreatedDate = n.CreatedDate
                        }).ToListAsync();
        }
    }
}