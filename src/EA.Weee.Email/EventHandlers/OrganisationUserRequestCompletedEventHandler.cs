namespace EA.Weee.Email.EventHandlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Events;
    using EA.Weee.Domain.Organisation;

    public class OrganisationUserRequestCompletedEventHandler : IEventHandler<OrganisationUserRequestCompletedEvent>
    {
        private IWeeeEmailService emailService;
        private readonly IOrganisationUserRequestEventHandlerDataAccess dataAccess;

        public OrganisationUserRequestCompletedEventHandler(IOrganisationUserRequestEventHandlerDataAccess dataAccess, IWeeeEmailService emailService)
        {
            this.emailService = emailService;
            this.dataAccess = dataAccess;
        }

        public async Task HandleAsync(OrganisationUserRequestCompletedEvent @event)
        {
            IEnumerable<OrganisationUser> activeOrganisationUsers = await dataAccess.FetchActiveOrganisationUsers(@event.OrganisationUser.OrganisationId);
            bool activeUsers = true;

            if (activeOrganisationUsers == null || !activeOrganisationUsers.Any())
            {
                activeUsers = false;
            }

            await emailService.SendOrganisationUserRequestCompleted(@event.OrganisationUser, activeUsers);
        }
    }
}
