namespace EA.Weee.RequestHandlers.Organisations.GetOrganisationOverview.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Weee.DataAccess;

    public class GetOrganisationOverviewDataAccess : IGetOrganisationOverviewDataAccess
    {
        private WeeeContext context;

        public GetOrganisationOverviewDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<bool> HasMemberSubmissions(Guid organisationId)
        {
            var query =
                from schemes in context.Schemes
                join organisations in context.Organisations on schemes.OrganisationId equals organisations.Id
                join memberUploads in context.MemberUploads on schemes.Id equals memberUploads.SchemeId
                where organisations.Id == organisationId && memberUploads.IsSubmitted
                select schemes;

            return await query.AnyAsync();
        }

        public async Task<bool> HasMultipleManageableOrganisationUsers(Guid organisationId)
        {
            return await context.OrganisationUsers.CountAsync(o => o.OrganisationId == organisationId &&
                                                              o.UserStatus.Value != UserStatus.Rejected.Value) > 1;
        }
    }
}
