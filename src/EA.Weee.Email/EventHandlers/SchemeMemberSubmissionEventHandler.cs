namespace EA.Weee.Email.EventHandlers
{
    using System.Threading.Tasks;
    using Domain.Events;
    using Prsd.Core.Domain;

    public class SchemeMemberSubmissionEventHandler : IEventHandler<SchemeMemberSubmissionEvent>
    {
        private readonly IWeeeNotificationEmailService notificationEmailService;

        public SchemeMemberSubmissionEventHandler(IWeeeNotificationEmailService notificationEmailService)
        {
            this.notificationEmailService = notificationEmailService;
        }

        public Task HandleAsync(SchemeMemberSubmissionEvent @event)
        {
            var memberUpload = @event.MemberUpload;

            return notificationEmailService.SendSchemeMemberSubmitted(
                memberUpload.Scheme.CompetentAuthority.Email,
                memberUpload.Scheme.SchemeName,
                memberUpload.ComplianceYear.Value,
                memberUpload.GetNumberOfWarnings());
        }
    }
}
