namespace EA.Weee.Requests.Tests.Unit.Organisations.Create
{
    using Domain;
    using RequestHandlers.Mappings;
    using Requests.Organisations.Create;
    using Xunit;

    public class CreateSoleTraderRequestMapTests
    {
        [Fact]
        public void MapFullyPopulatedCreateSoleTraderRequest_SetsTypeToSoleTrader()
        {
            const string tradingName = "test trading name";

            var mapper = new CreateSoleTraderRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreateSoleTraderRequest(tradingName));

            Assert.Equal(OrganisationType.SoleTraderOrIndividual, organisation.OrganisationType);
        }

        [Fact]
        public void MapFullyPopulatedCreateSoleTraderRequest_MapsAllProperties()
        {
            const string tradingName = "test trading name";

            var mapper = new CreateSoleTraderRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreateSoleTraderRequest(tradingName));

            Assert.Equal(tradingName, organisation.TradingName);
        }

        private CreateSoleTraderRequest FullyPopulatedCreateSoleTraderRequest(string tradingName)
        {
            return new CreateSoleTraderRequest
            {
                TradingName = tradingName,
            };
        }
    }
}
