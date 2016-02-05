namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.ContactDetails
{
    using System;
    using Web.ViewModels.OrganisationRegistration;
    using Web.ViewModels.Shared;

    public class ContactDetailsOverviewViewModel : OverviewViewModel
    {
        public ContactPersonViewModel Contact { get; set; }

        public AddressViewModel Address { get; set; }

        public ContactDetailsOverviewViewModel()
            : base(OverviewDisplayOption.ContactDetails)
        {
        }
    }
}