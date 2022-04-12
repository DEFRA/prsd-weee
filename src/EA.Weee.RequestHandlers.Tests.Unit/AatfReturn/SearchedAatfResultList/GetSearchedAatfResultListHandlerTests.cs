namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.SearchedAatfResultList
{
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.SearchedAatfResultList;
    using Xunit;

    public class GetSearchedAatfResultListHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISearchedAatfResultListDataAccess searchedAatfResultListDataAccess;
        private readonly GetSearchedAatfResultListHandler handler;

        public GetSearchedAatfResultListHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.searchedAatfResultListDataAccess = A.Fake<ISearchedAatfResultListDataAccess>();

            handler = new GetSearchedAatfResultListHandler(authorization, searchedAatfResultListDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetSearchedAatfResultListHandler(authorization, searchedAatfResultListDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetAatfAddressBySearchId>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetAnAatfBySearchIdRequest_DataAccessIsCalled()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var getSearchedAatfAddress = new GetAatfAddressBySearchId(aatfId);

            await handler.HandleAsync(getSearchedAatfAddress);

            A.CallTo(() => searchedAatfResultListDataAccess.GetAnAatfBySearchId(getSearchedAatfAddress.SearchedAatfId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenGetAnAatfBySearchId_ReturnedListShouldContainElements()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var getSearchAatfAddress = new GetAatfAddressBySearchId(aatfId);            

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() },
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1235RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() },
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1236RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
            };

            A.CallTo(() => searchedAatfResultListDataAccess.GetAnAatfBySearchId(getSearchAatfAddress.SearchedAatfId)).Returns(returnAatfAddressResult);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(3);
        }

        [Fact]
        public async void HandleAsync_GivenGetAnAatfBySearchId_ReturnedListShouldContainsOnly1Element()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var selectedAatfId = Guid.NewGuid();
            var getSearchAatfAddress = new GetAatfAddressBySearchId(aatfId);

            var returnAatfAddressResult = new List<WeeeSearchedAnAatfListData>()
            {
                new WeeeSearchedAnAatfListData() { ApprovalNumber = "WEE/QW1234RE/ATF", WeeeSentOnId = selectedAatfId, OperatorAddress = new AatfAddressData(), SiteAddress = new AatfAddressData() }
            };

            A.CallTo(() => searchedAatfResultListDataAccess.GetAnAatfBySearchId(getSearchAatfAddress.SearchedAatfId)).Returns(returnAatfAddressResult);

            var result = await handler.HandleAsync(getSearchAatfAddress);

            result.Count.Should().Be(1);
        }
    }
}
