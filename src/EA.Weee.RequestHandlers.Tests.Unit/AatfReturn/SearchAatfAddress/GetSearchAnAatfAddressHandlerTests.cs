namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.SearchAatfAddress
{
    using Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.SearchAatfAddress;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using Xunit;

    public class GetSearchAnAatfAddressHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISearchAnAatfAddressDataAccess getSearchAnAatfAddressDataAccess;
        private readonly GetSearchAnAatfAddressHandler handler;

        public GetSearchAnAatfAddressHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.getSearchAnAatfAddressDataAccess = A.Fake<ISearchAnAatfAddressDataAccess>();

            handler = new GetSearchAnAatfAddressHandler(authorization, getSearchAnAatfAddressDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetSearchAnAatfAddressHandler(authorization, getSearchAnAatfAddressDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSearchAatfAddress>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSearchAatfAddressRequest_DataAccessIsCalled()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var getSearchAatfAddress = new GetSearchAatfAddress("Test", aatfId);

            await handler.HandleAsync(getSearchAatfAddress);

            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenGetSearchAatfAddress_ReturnedListShouldContainElements()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var getSearchAatfAddress = new GetSearchAatfAddress("Test", aatfId);
            DateTime date = DateTime.Now;

            Aatf aatf1 = new Aatf("Test 1", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), AatfSize.Large, date, A.Fake<AatfContact>(), FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());
            Aatf aatf2 = new Aatf("Test 2", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), AatfSize.Large, date, A.Fake<AatfContact>(), FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());
            Aatf aatf3 = new Aatf("Test 3", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), AatfSize.Large, date, A.Fake<AatfContact>(), FacilityType.Ae, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());

            var aatfs = new List<Aatf>()
            {
               aatf1, aatf2, aatf3
            };

            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).Returns(aatfs);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(3);
        }

        [Fact]
        public async void HandleAsync_GivenGetSearchAatfAddress_ReturnedListShouldContainsOnly1Element()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var getSearchAatfAddress = new GetSearchAatfAddress("Test 1", aatfId);
            DateTime date = DateTime.Now;

            Aatf aatf1 = new Aatf("Test 1", A.Dummy<UKCompetentAuthority>(), "1234", AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), AatfSize.Large, date, A.Fake<AatfContact>(), FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());            

            var aatfs = new List<Aatf>()
            {
               aatf1
            };
            
            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).Returns(aatfs);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(1);
        }
    }
}
