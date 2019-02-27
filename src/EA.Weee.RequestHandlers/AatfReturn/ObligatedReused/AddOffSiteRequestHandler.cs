namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class AddOffSiteRequestHandler : IRequestHandler<AddOffSite, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddOffSiteDataAccess offSiteDataAccess;

        public AddOffSiteRequestHandler(IWeeeAuthorization authorization,
            IAddOffSiteDataAccess offSiteDataAccess)
        {
            this.authorization = authorization;
            this.offSiteDataAccess = offSiteDataAccess;
        }

        public async Task<bool> HandleAsync(AddOffSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var address = new Address(
                message.AddressData.Name,
                message.AddressData.Address1,
                message.AddressData.Address2,
                message.AddressData.TownOrCity,
                message.AddressData.CountyOrRegion,
                message.AddressData.Postcode,
                message.AddressData.CountryId);

            var aatfWeeeReused = new WeeeReusedSite(
                message.WeeeReused,
                address);

            await offSiteDataAccess.Submit(address);

            return true;
        }
    }
}
