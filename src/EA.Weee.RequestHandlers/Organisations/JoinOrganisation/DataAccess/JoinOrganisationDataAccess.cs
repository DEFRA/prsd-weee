namespace EA.Weee.RequestHandlers.Organisations.JoinOrganisation.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using Weee.DataAccess;

    public class JoinOrganisationDataAccess : IJoinOrganisationDataAccess
    {
        private readonly WeeeContext context;

        public JoinOrganisationDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<bool> DoesUserExist(Guid userId)
        {
            return await context.Users.AnyAsync(u => u.Id == userId.ToString());
        }

        public async Task<bool> DoesOrganisationExist(Guid organisationId)
        {
            return await context.Organisations.AnyAsync(o => o.Id == organisationId);
        }

        public async Task<JoinOrganisationResult> JoinOrganisation(OrganisationUser organisationUser)
        {
            var existingRequest = await context.OrganisationUsers
                .FirstOrDefaultAsync(ou => ou.OrganisationId == organisationUser.OrganisationId
                                && ou.UserId == organisationUser.UserId
                                && ou.UserStatus.Value != UserStatus.Rejected.Value);

            if (existingRequest == null)
            {
                context.OrganisationUsers.Add(organisationUser);
                await context.SaveChangesAsync();

                return JoinOrganisationResult.Success();
            }

            return JoinOrganisationResult.Fail(string.Format(
                "Cannot submit a request for user with Id '{0}' to access organisation with Id '{1}': A request to access this organisation already exists with state '{2}'",
                organisationUser.UserId,
                organisationUser.OrganisationId,
                existingRequest.UserStatus.DisplayName));
        }
    }
}
