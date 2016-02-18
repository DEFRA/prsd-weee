namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.OrganisationDetails
{
    using System;
    using Web.ViewModels.Shared;

    public abstract class OrganisationDetailsOverviewViewModel : OverviewViewModel
    {
        public string BusinessTradingName { get; set; }

        public AddressViewModel Address { get; set; }

        public bool CanEditOrganisation { get; set; }

        protected OrganisationDetailsOverviewViewModel()
            : base(OverviewDisplayOption.OrganisationDetails)
        {
            Address = new AddressViewModel();
        }
    }
}