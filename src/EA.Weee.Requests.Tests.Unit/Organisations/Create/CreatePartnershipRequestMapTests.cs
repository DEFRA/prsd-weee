namespace EA.Weee.Requests.Tests.Unit.Organisations.Create
{
    using Domain;
    using RequestHandlers.Mappings;
    using Requests.Organisations.Create;
    using Xunit;

    public class CreatePartnershipRequestMapTests
    {
        [Fact]
        public void MapFullyPopulatedCreatePartnershipRequest_SetsTypeToPartnership()
        {
            const string tradingName = "test trading name";

            var mapper = new CreatePartnershipRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreatePartnershipRequest(tradingName));

            Assert.Equal(OrganisationType.Partnership, organisation.OrganisationType);
        }

        [Fact]
        public void MapFullyPopulatedCreatePartnershipRequest_SetsStatusToIncomplete()
        {
            const string tradingName = "test trading name";

            var mapper = new CreatePartnershipRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreatePartnershipRequest(tradingName));

            Assert.Equal(OrganisationStatus.Incomplete, organisation.OrganisationStatus);
        }

        [Fact]
        public void MapFullyPopulatedCreatePartnershipRequest_MapsAllProperties()
        {
            const string tradingName = "test trading name";

            var mapper = new CreatePartnershipRequestMap();
            var organisation = mapper.Map(FullyPopulatedCreatePartnershipRequest(tradingName));

            Assert.Equal(tradingName, organisation.TradingName);
        }

        private CreatePartnershipRequest FullyPopulatedCreatePartnershipRequest(string tradingName)
        {
            return new CreatePartnershipRequest
            {
                TradingName = tradingName,
            };
        }
    }
}
