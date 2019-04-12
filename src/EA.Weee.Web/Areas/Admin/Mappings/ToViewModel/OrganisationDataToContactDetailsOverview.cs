namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.Organisations;
    using Core.Scheme;
    using Prsd.Core.Mapper;
    using ViewModels.Scheme.Overview.ContactDetails;
    using Web.ViewModels.OrganisationRegistration;
    using Web.ViewModels.Shared;

    public class OrganisationDataToContactDetailsOverview : IMap<SchemeData, ContactDetailsOverviewViewModel>
    {
        public ContactDetailsOverviewViewModel Map(SchemeData source)
        {
            return new ContactDetailsOverviewViewModel
            {
                Contact = new ContactPersonViewModel(source.Contact)
                {
                    OrganisationId = source.OrganisationId
                },
                Address = new AddressViewModel
                {
                    Address = source.Address
                }
            };
        }
    }
}