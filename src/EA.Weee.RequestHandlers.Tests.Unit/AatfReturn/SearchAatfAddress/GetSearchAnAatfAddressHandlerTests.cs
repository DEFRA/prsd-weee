namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.SearchAatfAddress
{
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
            var getSearchAatfAddress = new GetSearchAatfAddress("Test", aatfId, returnId);

            await handler.HandleAsync(getSearchAatfAddress);

            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenGetSearchAatfAddress_ReturnedListShouldContainElements()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            DateTime date = DateTime.Now;
            var getSearchAatfAddress = new GetSearchAatfAddress("Test", aatfId, returnId);

            List<Core.AatfReturn.ReturnAatfAddressResult> returnAatfAddressResults = new List<Core.AatfReturn.ReturnAatfAddressResult>();
            returnAatfAddressResults.Add(new Core.AatfReturn.ReturnAatfAddressResult() { OrganisationId = Guid.NewGuid(), SearchTermId = Guid.NewGuid(), SearchTermName = "Test 1" });

            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).Returns(returnAatfAddressResults);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(1);
        }

        [Fact]
        public async void HandleAsync_GivenGetSearchAatfAddress_ReturnedListShouldContainsOnly1Element()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            DateTime date = DateTime.Now;

            var getSearchAatfAddress = new GetSearchAatfAddress("Test 1", aatfId, returnId);

            List<Core.AatfReturn.ReturnAatfAddressResult> returnAatfAddressResults = new List<Core.AatfReturn.ReturnAatfAddressResult>();
            returnAatfAddressResults.Add(new Core.AatfReturn.ReturnAatfAddressResult() { OrganisationId = Guid.NewGuid(), SearchTermId = Guid.NewGuid(), SearchTermName = "Test 1" });

            A.CallTo(() => getSearchAnAatfAddressDataAccess.GetSearchAnAatfAddressBySearchTerm(getSearchAatfAddress)).Returns(returnAatfAddressResults);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(1);
        }
    }
}
