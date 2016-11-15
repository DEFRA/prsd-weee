namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Requests.Organisations;
    using Email;
    using Security;
    using Weee.Security;

    public class UpdateOrganisationContactDetailsHandler : IRequestHandler<UpdateOrganisationContactDetails, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDetailsDataAccess dataAccess;
        private readonly IWeeeEmailService weeeEmailService;

        public UpdateOrganisationContactDetailsHandler(IWeeeAuthorization authorization, IOrganisationDetailsDataAccess dataAccess,
            IWeeeEmailService weeeEmailService)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.weeeEmailService = weeeEmailService;
        }

        public async Task<bool> HandleAsync(UpdateOrganisationContactDetails message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationData.Id);
            if (authorization.CheckCanAccessInternalArea())
            {
                authorization.EnsureUserInRole(Roles.InternalAdmin);
            }

            Organisation organisation = await dataAccess.FetchOrganisationAsync(message.OrganisationData.Id);

            Contact contact = new Contact(
                message.OrganisationData.Contact.FirstName,
                message.OrganisationData.Contact.LastName,
                message.OrganisationData.Contact.Position);

            var contactChanged = !contact.Equals(organisation.Contact);

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

            var organisationAddressChanged = !address.Equals(organisation.OrganisationAddress);

            organisation.AddOrUpdateAddress(AddressType.OrganisationAddress, address);

            await dataAccess.SaveAsync();

            if (message.SendNotificationOnChange &&
                (contactChanged || organisationAddressChanged))
            {
                var scheme = await dataAccess.FetchSchemeAsync(message.OrganisationData.Id);

                if (scheme != null &&
                    scheme.CompetentAuthority != null)
                {
                    await weeeEmailService.SendOrganisationContactDetailsChanged(scheme.CompetentAuthority.Email, scheme.SchemeName);
                }
            }

            return true;
        }
    }
}
