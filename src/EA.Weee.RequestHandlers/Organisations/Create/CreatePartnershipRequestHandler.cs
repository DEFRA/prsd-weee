namespace EA.Weee.RequestHandlers.Organisations.Create
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using Domain.User;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations.Create;
    using Security;

    public class CreatePartnershipRequestHandler : IRequestHandler<CreatePartnershipRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext db;
        private readonly IUserContext userContext;

        public CreatePartnershipRequestHandler(IWeeeAuthorization authorization, WeeeContext db, IUserContext userContext)
        {
            this.authorization = authorization;
            this.db = db;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(CreatePartnershipRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = Organisation.CreatePartnership(message.TradingName);
            db.Organisations.Add(organisation);

            await db.SaveChangesAsync();

            var organisationUser = new OrganisationUser(userContext.UserId, organisation.Id, UserStatus.Active);
            db.OrganisationUsers.Add(organisationUser);

            await db.SaveChangesAsync();

            return organisation.Id;
        }
    }
}
