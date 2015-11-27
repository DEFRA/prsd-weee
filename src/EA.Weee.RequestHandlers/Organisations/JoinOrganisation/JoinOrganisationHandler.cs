namespace EA.Weee.RequestHandlers.Organisations.JoinOrganisation
{
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class JoinOrganisationHandler : IRequestHandler<JoinOrganisation, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IJoinOrganisationDataAccess dataAccess;
        private readonly IUserContext userContext;

        public JoinOrganisationHandler(IWeeeAuthorization authorization, IJoinOrganisationDataAccess dataAccess, IUserContext userContext)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(JoinOrganisation message)
        {
            authorization.EnsureCanAccessExternalArea();

            var userId = userContext.UserId;

            if (!await dataAccess.DoesUserExist(userId))
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}", userId));
            }

            if (!await dataAccess.DoesOrganisationExist(message.OrganisationId))
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    message.OrganisationId));
            }

            var organisationUser = new OrganisationUser(userId, message.OrganisationId,
                Domain.UserStatus.Pending);

            var result = await dataAccess.JoinOrganisation(organisationUser);

            if (!result.Successful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            return message.OrganisationId;
        }
    }
}
