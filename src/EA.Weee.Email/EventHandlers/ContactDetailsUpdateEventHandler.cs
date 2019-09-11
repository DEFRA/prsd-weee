namespace EA.Weee.Email.EventHandlers
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Events;

    public class ContactDetailsUpdateEventHandler : IEventHandler<AatfContactDetailsUpdateEvent>
    {
        private readonly IWeeeEmailService emailService;

        public ContactDetailsUpdateEventHandler(IWeeeEmailService emailService)
        {
            this.emailService = emailService;
        }

        public Task HandleAsync(AatfContactDetailsUpdateEvent @event)
        {
            return this.emailService.SendOrganisationContactDetailsChanged(@event.Aatf.CompetentAuthority.Email, @event.Aatf.Name, (EntityType)@event.Aatf.FacilityType.Value);
        }
    }
}
