namespace EA.Weee.RequestHandlers.Scheme
{
    using System.Threading.Tasks;
    using Domain;
    using Domain.Organisation;
    using Domain.Scheme;
    using Email;
    using Organisations;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Scheme;
    using Security;
    using Weee.Security;

    public class UpdateSchemeContactDetailsHandler : IRequestHandler<UpdateSchemeContactDetails, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDetailsDataAccess dataAccess;
        private readonly IWeeeEmailService weeeEmailService;

        public UpdateSchemeContactDetailsHandler(IWeeeAuthorization authorization, IOrganisationDetailsDataAccess dataAccess,
            IWeeeEmailService weeeEmailService)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.weeeEmailService = weeeEmailService;
        }

        public async Task<bool> HandleAsync(UpdateSchemeContactDetails message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationData.Id);
            if (authorization.CheckCanAccessInternalArea())
            {
                authorization.EnsureUserInRole(Roles.InternalAdmin);
            }

            var scheme = await dataAccess.FetchSchemeAsync(message.OrganisationData.Id);
            var organisation = await dataAccess.FetchOrganisationAsync(message.OrganisationData.Id);

            var contact = new Contact(
                message.OrganisationData.Contact.FirstName,
                message.OrganisationData.Contact.LastName,
                message.OrganisationData.Contact.Position);

            var contactChanged = !contact.Equals(scheme.Contact);

            scheme.AddOrUpdateMainContactPerson(contact);

            var country = await dataAccess.FetchCountryAsync(message.OrganisationData.OrganisationAddress.CountryId);

            var address = new Address(
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
                scheme = await dataAccess.FetchSchemeAsync(message.OrganisationData.Id);

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
