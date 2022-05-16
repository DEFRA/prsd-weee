namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class EditAatfSiteHandler : IRequestHandler<EditAatfSite, bool>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public EditAatfSiteHandler(WeeeContext context,
            IWeeeAuthorization authorization,
            IAatfSiteDataAccess offSiteDataAccess,
            IGenericDataAccess genericDataAccess,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.offSiteDataAccess = offSiteDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
        }

        public async Task<bool> HandleAsync(EditAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.AddressData.CountryId);

            var value = await genericDataAccess.GetById<AatfAddress>(message.AddressData.Id);

            await offSiteDataAccess.Update(value, message.AddressData, country);

            return true;
        }
    }
}
