namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class AddSchemeViewModel : SchemeViewModelBase
    {
        public AddressData OrganisationAddress { get; set; }

        public ContactData Contact { get; set; }

        public string OrganisationName { get; set; }
    }
}