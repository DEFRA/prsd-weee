namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    
    internal class CompleteRegistrationHandler : IRequestHandler<CompleteRegistration, Guid>
    {
        private readonly WeeeContext db;

        private readonly IUserContext userContext;

        public CompleteRegistrationHandler(WeeeContext context, IUserContext userContext)
        {
            db = context;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CompleteRegistration message)
        {
            var userId = userContext.UserId;

            if (await db.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString()) == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", userId));
            }

            if (await db.Organisations.FirstOrDefaultAsync(o => o.Id == message.OrganisationId) == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}", message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(userId, message.OrganisationId, EA.Weee.Domain.OrganisationUserStatus.Approved);

            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            db.OrganisationUsers.Add(organisationUser);
            organisation.CompleteRegistration();

            await db.SaveChangesAsync();
            return organisation.Id;
        }
    }
}
