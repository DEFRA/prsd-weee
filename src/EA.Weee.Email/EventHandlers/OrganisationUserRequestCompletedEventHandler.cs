namespace EA.Weee.Email.EventHandlers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Events;
    using System.Threading.Tasks;

    public class OrganisationUserRequestCompletedEventHandler : IEventHandler<OrganisationUserRequestCompletedEvent>
    {
        private IWeeeEmailService emailService;

        public OrganisationUserRequestCompletedEventHandler(IWeeeEmailService emailService)
        {
            this.emailService = emailService;
        }

        public async Task HandleAsync(OrganisationUserRequestCompletedEvent @event)
        {
            await emailService.SendOrganisationUserRequestCompleted(@event.OrganisationUser);
        }
    }
}
