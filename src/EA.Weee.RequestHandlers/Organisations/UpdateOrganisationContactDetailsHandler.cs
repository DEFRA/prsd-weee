namespace EA.Weee.RequestHandlers.Organisations
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests;
    using EA.Weee.Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateOrganisationContactDetailsHandler : IRequestHandler<UpdateOrganisationContactDetails, UpdateResult>
    {
        private IUpdateOrganisationContactDetailsDataAccess dataAccess;
        private IMap<Organisation, OrganisationData> mapping;

        public UpdateOrganisationContactDetailsHandler(IUpdateOrganisationContactDetailsDataAccess dataAccess, IMap<Organisation, OrganisationData> mapping)
        {
            this.dataAccess = dataAccess;
            this.mapping = mapping;
        }

        public async Task<UpdateResult> HandleAsync(UpdateOrganisationContactDetails message)
        {
            Organisation organisation = await dataAccess.FetchOrganisationAsync(
                message.OrganisationData.Id,
                message.OrganisationData.Contact.RowVersion,
                message.OrganisationData.OrganisationAddress.RowVersion);

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

            try
            {
                await dataAccess.SaveAsync();
                return UpdateResult.OK;
            }
            catch (ConcurrencyException)
            {
                return UpdateResult.ConcurrencyError;
            }
            catch (Exception)
            {
                return UpdateResult.UnknownError;
            }
        }
    }
}
