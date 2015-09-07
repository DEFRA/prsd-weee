namespace EA.Weee.Email.EventHandlers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
            IEnumerable<User> recipients = await dataAccess.FetchActiveOrganisationUsers(@event.OrganisationUser.OrganisationId);

            List<Task> sendEmailTasks = new List<Task>();
            foreach (User recipient in recipients)
            {
                sendEmailTasks.Add(emailService.SendOrganisationUserRequest(recipient.Email, @event.OrganisationUser));
            }

            foreach (Task sendEmailTask in sendEmailTasks)
            {
                await sendEmailTask;
            }
        }
    }
}
