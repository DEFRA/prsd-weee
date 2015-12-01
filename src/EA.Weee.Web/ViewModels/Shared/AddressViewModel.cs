namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using Core.Organisations;
    using Core.Shared;
    using Weee.Requests.Organisations;

    public class AddressViewModel
    {
        public AddressViewModel()
        {
            Address = new AddressData();
        }

        public Guid OrganisationId { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public AddressData Address { get; set; }

        public AddAddressToOrganisation ToAddRequest(AddressType type)
        {
            return new AddAddressToOrganisation(OrganisationId, type, Address);
        }
    }
}