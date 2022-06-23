namespace EA.Weee.RequestHandlers.Aatf
{
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Events;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Aatf;
    using EA.Weee.Security;

    internal class EditAatfContactHandler : IRequestHandler<EditAatfContact, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditAatfContactHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IGenericDataAccess genericDataAccess,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
        }

        public async Task<bool> HandleAsync(EditAatfContact message)
        {
            var aatf = await this.aatfDataAccess.GetDetails(message.AatfId);

            this.authorization.EnsureInternalOrOrganisationAccess(aatf.Organisation.Id);

            if (this.authorization.CheckCanAccessInternalArea())
            {
                this.authorization.EnsureUserInRole(Roles.InternalAdmin);
            }

            var aatfContact = this.GetAatfContactToCompare(message);

            if (message.SendNotification && !aatf.Contact.Equals(aatfContact))
            {
                if (!await this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId))
                {
                    throw new InvalidOperationException($"Contact record cannot be updated {message.ContactData.Id}");
                }

                aatf.RaiseEvent(new AatfContactDetailsUpdateEvent(aatf));
            }

            var country = await this.organisationDetailsDataAccess.FetchCountryAsync(message.ContactData.AddressData.CountryId);

            var value = await this.genericDataAccess.GetById<AatfContact>(message.ContactData.Id);

            await this.aatfDataAccess.UpdateContact(value, message.ContactData, country);

            return true;
        }

        private AatfContact GetAatfContactToCompare(EditAatfContact message)
        {
            return new AatfContact(message.ContactData.FirstName, 
                message.ContactData.LastName, 
                message.ContactData.Position, 
                message.ContactData.AddressData.Address1, 
                message.ContactData.AddressData.Address2, 
                message.ContactData.AddressData.TownOrCity, 
                message.ContactData.AddressData.CountyOrRegion, 
                message.ContactData.AddressData.Postcode, 
                message.ContactData.AddressData.CountryId, 
                message.ContactData.Telephone, 
                message.ContactData.Email);
        }
    }
}
