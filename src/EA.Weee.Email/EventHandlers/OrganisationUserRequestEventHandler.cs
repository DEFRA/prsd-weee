namespace EA.Weee.Email.EventHandlers
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Events;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrganisationUserRequestEventHandler : IEventHandler<OrganisationUserRequestEvent>
    {
        private readonly WeeeContext context;
        private readonly IWeeeEmailService emailService;

        public OrganisationUserRequestEventHandler(WeeeContext context, IWeeeEmailService emailService)
        {
            this.context = context;
            this.emailService = emailService;
        }

        public async Task HandleAsync(OrganisationUserRequestEvent @event)
        {
            List<User> recipients = await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == @event.OrganisationUser.OrganisationId)
                .Where(ou => ou.UserStatus.Value == UserStatus.Active.Value)
                .Select(ou => ou.User)
                .Distinct()
                .OrderBy(u => u.Email)
                .ToListAsync();

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
