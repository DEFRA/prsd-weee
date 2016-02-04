namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.OrganisationDetails
{
    using System;
    using Core.Organisations;

    public class SoleTraderDetailsOverviewViewModel : OrganisationDetailsOverviewViewModel
    {
        public SoleTraderDetailsOverviewViewModel(Guid schemeId, string schemeName) : base(schemeId, schemeName)
        {
        }
    }
}