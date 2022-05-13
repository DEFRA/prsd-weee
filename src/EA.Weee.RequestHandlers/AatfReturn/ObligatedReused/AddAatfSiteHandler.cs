namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class AddAatfSiteHandler : IRequestHandler<AddAatfSite, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfSiteDataAccess offSiteDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;

        public AddAatfSiteHandler(IWeeeAuthorization authorization,
            IAatfSiteDataAccess offSiteDataAccess, IGenericDataAccess genericDataAccess,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess)
        {
            this.authorization = authorization;
            this.offSiteDataAccess = offSiteDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
        }

        public async Task<bool> HandleAsync(AddAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            Country country = await organisationDetailsDataAccess.FetchCountryAsync(message.AddressData.CountryId);

            var address = new AatfAddress(
                message.AddressData.Name,
                message.AddressData.Address1,
                message.AddressData.Address2,
                message.AddressData.TownOrCity,
                message.AddressData.CountyOrRegion,
                message.AddressData.Postcode,
                country);

            var weeeReused = await genericDataAccess.GetManyByExpression<WeeeReused>(new WeeeReusedByAatfIdAndReturnIdSpecification(message.AatfId, message.ReturnId));

            var weeeReusedSite = new WeeeReusedSite(
                weeeReused.Last(),
                address);

            await offSiteDataAccess.Submit(weeeReusedSite);

            return true;
        }
    }
}
