namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;

    public class GetSentOnAatfSiteHandler : IRequestHandler<GetSentOnAatfSite, AatfAddressData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddSentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IMap<AatfAddress, AddressData> mapper;

        public GetSentOnAatfSiteHandler(IWeeeAuthorization authorization,
            IAddSentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess, IMap<AatfAddress, AddressData> mapper)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.mapper = mapper;
        }

        public async Task<AatfAddressData> HandleAsync(GetSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfAddress = await getSentOnAatfSiteDataAccess.GetWeeeSentOnAddress(message.WeeeSentOnId);

            var addressData = mapper.Map(aatfAddress);

            var aatfAddressData = new AatfAddressData()
            {
                Address1 = addressData.Address1,
                Address2 = addressData.Address2,
                TownOrCity = addressData.TownOrCity,
                CountryId = addressData.CountryId,
                CountryName = addressData.CountryName,
                Countries = addressData.Countries,
                CountyOrRegion = addressData.CountyOrRegion,
                Name = addressData.Name,
                Postcode = addressData.Postcode
            };

            return aatfAddressData;
        }
    }
}
