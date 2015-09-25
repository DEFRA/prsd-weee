namespace EA.Weee.RequestHandlers.Users
{
    using DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Users;
    using Security;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class UpdateOrganisationUserStatusHandler : IRequestHandler<UpdateOrganisationUserStatus, int>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        public UpdateOrganisationUserStatusHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.authorization = authorization;
        }

        public async Task<int> HandleAsync(UpdateOrganisationUserStatus query)
        {
            OrganisationUser organisationUser = await context.OrganisationUsers.FindAsync(query.OrganisationUserId);

            if (organisationUser == null)
            {
                string message = string.Format(
                    "No organisation user was found with ID \"{0}\".",
                    query.OrganisationUserId);
                throw new Exception(message);
            }

            // TODO : Needs to be changed this authorisation code as per 34415
            //authorization.EnsureOrganisationAccess(organisationUser.OrganisationId);
            
            UserStatus userStatus = ValueObjectInitializer.GetUserStatus(query.UserStatus);
            
            organisationUser.UpdateUserStatus(userStatus);
            
            await context.SaveChangesAsync();
            
            return 0;
        }
    }
}
