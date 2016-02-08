namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Areas.Admin.ViewModels.Scheme.Overview.PcsDetails;
    using Core.Scheme;
    using Core.Shared;
    using Prsd.Core.Mapper;

    public class SchemeDataToPcsDetailsOverviewViewModel : IMap<SchemeData, PcsDetailsOverviewViewModel>
    {
        public PcsDetailsOverviewViewModel Map(SchemeData source)
        {
            return new PcsDetailsOverviewViewModel
            {
                AppropriateAuthority = source.CompetentAuthority != null 
                    ? source.CompetentAuthority.Name 
                    : string.Empty,
                ApprovalNumber = source.ApprovalName,
                BillingReference = source.IbisCustomerReference,
                ObligationType = source.ObligationType.HasValue
                    ? source.ObligationType.Value.ToString()
                    : string.Empty,
                Status = source.SchemeStatus.ToString(),
                SchemeId = source.Id,
                SchemeName = source.SchemeName,
                IsRejected = source.SchemeStatus == SchemeStatus.Rejected
            };
        }
    }
}