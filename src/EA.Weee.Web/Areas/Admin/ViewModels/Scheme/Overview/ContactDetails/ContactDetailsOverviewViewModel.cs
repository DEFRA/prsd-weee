namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview.ContactDetails
{
    using System;

    public class ContactDetailsOverviewViewModel : OverviewViewModel
    {
        public Guid OrganisationId { get; set; }

        public ContactDetailsOverviewViewModel(Guid schemeId, string schemeName)
            : base(schemeId, schemeName, OverviewDisplayOption.ContactDetails)
        {
        }
    }
}