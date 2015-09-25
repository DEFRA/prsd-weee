namespace EA.Weee.RequestHandlers.Admin
{
    using DataAccess;
    using EA.Weee.Domain;
    using Mappings;
    using Prsd.Core.Mediator;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Requests.Admin;

    internal class UpdateCompetentAuthorityUserStatusHandler : IRequestHandler<UpdateCompetentAuthorityUserStatus, Guid>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public UpdateCompetentAuthorityUserStatusHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateCompetentAuthorityUserStatus query)
        {
            authorization.EnsureCanAccessInternalArea();

            var competentAuthorityUser = await context.CompetentAuthorityUsers.FindAsync(query.Id);

            if (competentAuthorityUser == null)
            {
                string message = string.Format(
                    "No competent authority user was found with ID \"{0}\".",
                    query.Id);
                throw new Exception(message);
            }

            UserStatus userStatus = ValueObjectInitializer.GetUserStatus(query.UserStatus);

            competentAuthorityUser.UpdateUserStatus(userStatus);

            await context.SaveChangesAsync();

            return competentAuthorityUser.CompetentAuthorityId;
        }
    }
}
