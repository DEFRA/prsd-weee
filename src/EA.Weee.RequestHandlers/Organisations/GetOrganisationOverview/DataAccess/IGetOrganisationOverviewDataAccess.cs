namespace EA.Weee.RequestHandlers.Organisations.GetOrganisationOverview.DataAccess
{
    using System;
    using System.Threading.Tasks;

    public interface IGetOrganisationOverviewDataAccess
    {
        Task<bool> HasMultipleManageableOrganisationUsers(Guid organisationId);

        Task<bool> HasMemberSubmissions(Guid organisationId);

        Task<bool> HasDataReturnSubmissions(Guid organisationId);
    }
}