﻿namespace EA.Weee.RequestHandlers.Scheme
{
    using Domain.Organisation;
    using Email;
    using Organisations;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;
    using System;
    using System.Threading.Tasks;
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
            authorization.EnsureInternalOrOrganisationAccess(message.SchemeData.OrganisationId);
            if (authorization.CheckCanAccessInternalArea())
            {
                authorization.EnsureUserInRole(Roles.InternalAdmin);
            }

            var scheme = await dataAccess.FetchSchemeAsync(message.SchemeData.OrganisationId);

            if (scheme == null)
            {
                var errorMessage = $"A scheme with organisation id \"{message.SchemeData.OrganisationId}\" could not be found.";

                throw new ArgumentException(errorMessage);
            }

            var contact = new Contact(
                message.SchemeData.Contact.FirstName,
                message.SchemeData.Contact.LastName,
                message.SchemeData.Contact.Position);

            var contactChanged = !contact.Equals(scheme.Contact);

            scheme.AddOrUpdateMainContactPerson(contact);

            var country = await dataAccess.FetchCountryAsync(message.SchemeData.Address.CountryId);

            var address = new Address(
                message.SchemeData.Address.Address1,
                message.SchemeData.Address.Address2,
                message.SchemeData.Address.TownOrCity,
                message.SchemeData.Address.CountyOrRegion,
                message.SchemeData.Address.Postcode,
                country,
                message.SchemeData.Address.Telephone,
                message.SchemeData.Address.Email);

            var schemeAddressChanged = !address.Equals(scheme.Address);

            scheme.AddOrUpdateAddress(address);

            await dataAccess.SaveAsync();

            if (message.SendNotificationOnChange &&
                (contactChanged || schemeAddressChanged) && scheme.CompetentAuthority != null)
            {
                await weeeEmailService.SendOrganisationContactDetailsChanged(scheme.CompetentAuthority.Email, scheme.SchemeName);
            }

            return true;
        }
    }
}
