namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests.Organisations;
    using System.Threading.Tasks;

    public class UpdateOrganisationContactDetailsHandler : IRequestHandler<UpdateOrganisationContactDetails, bool>
    {
        private IUpdateOrganisationContactDetailsDataAccess dataAccess;

        public UpdateOrganisationContactDetailsHandler(IUpdateOrganisationContactDetailsDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(UpdateOrganisationContactDetails message)
        {
            Organisation organisation = await dataAccess.FetchOrganisationAsync(message.OrganisationData.Id);

            Contact contact = new Contact(
                message.OrganisationData.Contact.FirstName,
                message.OrganisationData.Contact.LastName,
                message.OrganisationData.Contact.Position);

            organisation.AddOrUpdateMainContactPerson(contact);

            Country country = await dataAccess.FetchCountryAsync(message.OrganisationData.OrganisationAddress.CountryId);

            Address address = new Address(
                message.OrganisationData.OrganisationAddress.Address1,
                message.OrganisationData.OrganisationAddress.Address2,
                message.OrganisationData.OrganisationAddress.TownOrCity,
                message.OrganisationData.OrganisationAddress.CountyOrRegion,
                message.OrganisationData.OrganisationAddress.Postcode,
                country,
                message.OrganisationData.OrganisationAddress.Telephone,
                message.OrganisationData.OrganisationAddress.Email);
            
            organisation.AddOrUpdateAddress(AddressType.OrganisationAddress, address);

            await dataAccess.SaveAsync();

            return true;
        }
    }
}
