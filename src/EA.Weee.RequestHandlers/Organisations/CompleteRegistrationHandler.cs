namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using OrganisationUserStatus = Domain.Organisation.OrganisationUserStatus;

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
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(userId, message.OrganisationId, OrganisationUserStatus.Approved);

            var organisation = await db.Organisations.SingleAsync(o => o.Id == message.OrganisationId);

            db.OrganisationUsers.Add(organisationUser);
            organisation.CompleteRegistration();
            
            //Created PCS created here until AATF/AE are impelemented
            var scheme = new Scheme(message.OrganisationId);
            db.Schemes.Add(scheme);
           
            await db.SaveChangesAsync();
            return organisation.Id;
        }
    }
}
