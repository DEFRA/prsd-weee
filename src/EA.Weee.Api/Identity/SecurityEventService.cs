namespace EA.Weee.Api.Identity
{
    using Thinktecture.IdentityServer.Core.Events;
    using Thinktecture.IdentityServer.Core.Services;

    /// <summary>
    /// An implmentation of Identity Servers's IEventService interface which routes
    /// login success and failure events to an ISecurityEventAuditor.
    /// </summary>
    public class SecurityEventService : IEventService
    {
        private readonly ISecurityEventAuditor auditSecurityEventService;

        public SecurityEventService(ISecurityEventAuditor auditSecurityEventService)
        {
            this.auditSecurityEventService = auditSecurityEventService;
        }

        public void Raise<T>(Thinktecture.IdentityServer.Core.Events.Event<T> evt)
        {
            Event<LocalLoginDetails> localLoginDetailsEvent = evt as Event<LocalLoginDetails>;
            if (localLoginDetailsEvent != null)
            {
                AuditLogin(localLoginDetailsEvent);
            }
        }

        private void AuditLogin(Event<LocalLoginDetails> localLoginDetailsEvent)
        {
            bool success = localLoginDetailsEvent.EventType == EventTypes.Success;

            if (success)
            {
                string userId = localLoginDetailsEvent.Details.SubjectId;
                auditSecurityEventService.LoginSuccess(userId);
            }
            else
            {
                string userName = localLoginDetailsEvent.Details.LoginUserName;
                auditSecurityEventService.LoginFailure(userName);
            }
        }
    }
}