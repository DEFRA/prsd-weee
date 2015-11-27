namespace EA.Weee.Email.EventHandlers
{
    using Domain.Events;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class OrganisationUserRequestEventHandler : IEventHandler<OrganisationUserRequestEvent>
    {
        private readonly IOrganisationUserRequestEventHandlerDataAccess dataAccess;
        private readonly IWeeeEmailService emailService;

        public OrganisationUserRequestEventHandler(IOrganisationUserRequestEventHandlerDataAccess dataAccess, IWeeeEmailService emailService)
        {
            this.dataAccess = dataAccess;
            this.emailService = emailService;
        }

        public async Task HandleAsync(OrganisationUserRequestEvent @event)
        {
            IEnumerable<OrganisationUser> activeOrganisationUsers = await dataAccess.FetchActiveOrganisationUsers(@event.OrganisationId);

            foreach (OrganisationUser activeOrganisationUser in activeOrganisationUsers)
            {
                await emailService.SendOrganisationUserRequest(activeOrganisationUser.User.Email, activeOrganisationUser.Organisation.OrganisationName);
            }
        }
    }
}
