namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.ContactDetails
{
    using Web.ViewModels.Shared;

    public class ContactDetailsOverviewViewModel : OverviewViewModel
    {
        public ContactPersonViewModel Contact { get; set; }

        public AddressViewModel Address { get; set; }

        public bool CanEditContactDetails { get; set; }

        public ContactDetailsOverviewViewModel()
            : base(OverviewDisplayOption.ContactDetails)
        {
        }
    }
}