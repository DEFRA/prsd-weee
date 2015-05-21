namespace EA.Weee.RequestHandlers.Notification
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.Notification;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Notification;
    using NotificationType = Requests.Shared.NotificationType;

    public class CreateNotificationApplicationHandler : IRequestHandler<CreateNotificationApplication, Guid>
    {
        private const string NotificationNumberSequenceFormat = "[Notification].[{0}NotificationNumber]";

        private readonly IwsContext context;
        private readonly IUserContext userContext;

        public CreateNotificationApplicationHandler(IwsContext context, IUserContext userContext)
        {
            this.context = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CreateNotificationApplication command)
        {
            UKCompetentAuthority authority;
            Domain.Notification.NotificationType notificationType;

            switch (command.CompetentAuthority)
            {
                case Requests.Notification.CompetentAuthority.England:
                    authority = UKCompetentAuthority.England;
                    break;
                case Requests.Notification.CompetentAuthority.NorthernIreland:
                    authority = UKCompetentAuthority.NorthernIreland;
                    break;
                case Requests.Notification.CompetentAuthority.Scotland:
                    authority = UKCompetentAuthority.Scotland;
                    break;
                case Requests.Notification.CompetentAuthority.Wales:
                    authority = UKCompetentAuthority.Wales;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown competent authority: {0}", command.CompetentAuthority));
            }

            switch (command.NotificationType)
            {
                case NotificationType.Recovery:
                    notificationType = Domain.Notification.NotificationType.Recovery;
                    break;
                case NotificationType.Disposal:
                    notificationType = Domain.Notification.NotificationType.Disposal;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown notification type: {0}", command.NotificationType));
            }

            var notificationNumber = await GetNextNotificationNumberAsync(authority);
            var notification = new NotificationApplication(userContext.UserId, notificationType, authority, notificationNumber);
            context.NotificationApplications.Add(notification);
            await context.SaveChangesAsync();
            return notification.Id;
        }

        private async Task<int> GetNextNotificationNumberAsync(UKCompetentAuthority authority)
        {
            return await context.Database.SqlQuery<int>(CreateSqlQuery(authority)).SingleAsync();
        }

        private static string CreateSqlQuery(UKCompetentAuthority competentAuthority)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT NEXT VALUE FOR ");
            stringBuilder.AppendFormat(NotificationNumberSequenceFormat, competentAuthority.ShortName);
            return stringBuilder.ToString();
        }
    }
}