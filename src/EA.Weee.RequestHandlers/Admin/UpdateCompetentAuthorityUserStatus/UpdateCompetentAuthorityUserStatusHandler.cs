namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    internal class UpdateCompetentAuthorityUserStatusHandler : IRequestHandler<UpdateCompetentAuthorityUserStatus, Guid>
    {
        private readonly IUpdateCompetentAuthorityUserStatusDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IUserContext userContext;

        public UpdateCompetentAuthorityUserStatusHandler(IUserContext userContext, IUpdateCompetentAuthorityUserStatusDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.userContext = userContext;
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateCompetentAuthorityUserStatus query)
        {
            authorization.EnsureCanAccessInternalArea();

            var competentAuthorityUser = await dataAccess.GetCompetentAuthorityUser(query.Id);

            if (competentAuthorityUser == null)
            {
                string message = string.Format(
                    "No competent authority user was found with ID \"{0}\".",
                    query.Id);
                throw new Exception(message);
            }

            if (userContext != null && userContext.UserId.ToString() == competentAuthorityUser.UserId)
            {
                throw new InvalidOperationException(string.Format("Error for user with Id '{0}': Users cannot change their own status", userContext.UserId));
            }

            await dataAccess.UpdateCompetentAuthorityUserStatus(competentAuthorityUser, query.UserStatus);

            return competentAuthorityUser.CompetentAuthorityId;
        }
    }
}
