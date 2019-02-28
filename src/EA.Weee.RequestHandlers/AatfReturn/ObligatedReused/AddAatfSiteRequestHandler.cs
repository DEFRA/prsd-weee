namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class AddAatfSiteRequestHandler : IRequestHandler<AddAatfSite, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddAatfSiteDataAccess offSiteDataAccess;

        public AddAatfSiteRequestHandler(IWeeeAuthorization authorization,
            IAddAatfSiteDataAccess offSiteDataAccess)
        {
            this.authorization = authorization;
            this.offSiteDataAccess = offSiteDataAccess;
        }

        public async Task<bool> HandleAsync(AddAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var address = new AatfAddress(
                message.AddressData.Name,
                message.AddressData.Address1,
                message.AddressData.Address2,
                message.AddressData.TownOrCity,
                message.AddressData.CountyOrRegion,
                message.AddressData.Postcode,
                message.AddressData.CountryId);

            var weeeReusedSite = new WeeeReusedSite(
                message.WeeeReusedId,
                address);

            await offSiteDataAccess.Submit(weeeReusedSite);

            return true;
        }
    }
}
