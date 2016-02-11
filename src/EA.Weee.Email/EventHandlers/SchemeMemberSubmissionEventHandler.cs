namespace EA.Weee.Email.EventHandlers
{
    using System.Threading.Tasks;
    using Domain.Events;
    using Prsd.Core.Domain;

    public class SchemeMemberSubmissionEventHandler : IEventHandler<SchemeMemberSubmissionEvent>
    {
        private readonly IWeeeEmailService weeeEmailService;

        public SchemeMemberSubmissionEventHandler(IWeeeEmailService weeeEmailService)
        {
            this.weeeEmailService = weeeEmailService;
        }

        public Task HandleAsync(SchemeMemberSubmissionEvent @event)
        {
            var memberUpload = @event.MemberUpload;

            return weeeEmailService.SendSchemeMemberSubmitted(
                memberUpload.Scheme.CompetentAuthority.Email,
                memberUpload.Scheme.SchemeName,
                memberUpload.ComplianceYear.Value,
                memberUpload.GetNumberOfWarnings());
        }
    }
}
