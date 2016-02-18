namespace EA.Weee.RequestHandlers.Organisations.GetOrganisationOverview.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.User;
    using Weee.DataAccess;

    public class GetOrganisationOverviewDataAccess : IGetOrganisationOverviewDataAccess
    {
        private readonly WeeeContext context;

        public GetOrganisationOverviewDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task<bool> HasDataReturnSubmissions(Guid organisationId)
        {
            return context.DataReturnVersions
                .Where(d => d.DataReturn.Scheme.OrganisationId == organisationId)
                .AnyAsync(d => d.SubmittedDate.HasValue);
        }

        public Task<bool> HasMemberSubmissions(Guid organisationId)
        {
            return context.MemberUploads.AnyAsync(
                            m => m.IsSubmitted &&
                            m.Organisation.Id == organisationId);
        }

        public async Task<bool> HasMultipleManageableOrganisationUsers(Guid organisationId)
        {
            return await context.OrganisationUsers.CountAsync(o => o.OrganisationId == organisationId &&
                                                              o.UserStatus.Value != UserStatus.Rejected.Value) > 1;
        }
    }
}
