namespace EA.Weee.RequestHandlers.Aatf
{
    using System;
    using System.Threading.Tasks;

    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Events;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
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

            if (message.SendNotification)
            {
                if (!await this.aatfDataAccess.IsLatestAatf(aatf.Id, aatf.AatfId))
                {
                    throw new InvalidOperationException($"Contact record cannot be updated {message.ContactData.Id}");
                }

                aatf.RaiseEvent(new AatfContactDetailsUpdateEvent(aatf));
                // TODO: Only create event on contact or address changing
            }

            var country = await this.organisationDetailsDataAccess.FetchCountryAsync(message.ContactData.AddressData.CountryId);

            var value = await this.genericDataAccess.GetById<AatfContact>(message.ContactData.Id);

            await this.aatfDataAccess.UpdateContact(value, message.ContactData, country);

            return true;
        }
    }
}
