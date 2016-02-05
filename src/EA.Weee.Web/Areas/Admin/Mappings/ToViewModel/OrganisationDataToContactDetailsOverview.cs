namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Organisations;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.ContactDetails;
    using Web.ViewModels.OrganisationRegistration;
    using Web.ViewModels.Shared;

    public class OrganisationDataToContactDetailsOverview : IMap<OrganisationData, ContactDetailsOverviewViewModel>
    {
        public ContactDetailsOverviewViewModel Map(OrganisationData source)
        {
            return new ContactDetailsOverviewViewModel
            {
                Contact = new ContactPersonViewModel(source.Contact)
                {
                    OrganisationId = source.Id
                },
                Address = new AddressViewModel
                {
                    Address = source.OrganisationAddress
                }
            };
        }
    }
}