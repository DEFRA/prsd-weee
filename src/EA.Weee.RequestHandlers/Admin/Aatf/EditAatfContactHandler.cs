namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Domain;
    using Domain.AatfReturn;
    using Organisations;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using Weee.Security;

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
            authorization.EnsureCanAccessInternalArea();
            if (authorization.CheckCanAccessInternalArea())
            {
                authorization.EnsureUserInRole(Roles.InternalAdmin);
            }

            Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.ContactData.AddressData.CountryId);

            var value = await genericDataAccess.GetById<AatfContact>(message.ContactData.Id);

            await aatfDataAccess.UpdateContact(value, message.ContactData, country);

            return true;
        }
    }
}
