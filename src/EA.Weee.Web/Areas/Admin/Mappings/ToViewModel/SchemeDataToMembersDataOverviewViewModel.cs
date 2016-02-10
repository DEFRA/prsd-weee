namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Scheme;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.MembersData;

    public class SchemeDataToMembersDataOverviewViewModel : IMap<SchemeData, MembersDataOverviewViewModel>
    {
        public MembersDataOverviewViewModel Map(SchemeData source)
        {
            return new MembersDataOverviewViewModel
            {
                SchemeId = source.Id,
                SchemeName = source.SchemeName,
                ApprovalNumber = source.ApprovalName,
                SchemeDataAvailability = source.SchemeDataAvailability,
                OrganisationId = source.OrganisationId
            };
        }
    }
}