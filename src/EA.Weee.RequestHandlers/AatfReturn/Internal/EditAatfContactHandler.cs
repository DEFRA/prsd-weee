namespace EA.Weee.RequestHandlers.AatfReturn.Internal
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Security;

    internal class EditAatfContactHandler : IRequestHandler<EditAatfContact, bool>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfContactDataAccess aatfContactDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditAatfContactHandler(WeeeContext context,
            IWeeeAuthorization authorization,
            IAatfContactDataAccess aatfContactDataAccess,
            IGenericDataAccess genericDataAccess,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.aatfContactDataAccess = aatfContactDataAccess;
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

            await aatfContactDataAccess.Update(value, message.ContactData, country);

            return true;
        }
    }
}
