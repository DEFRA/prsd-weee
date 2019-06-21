namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetWeeeSentOnHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly GetWeeeSentOnHandler handler;

        public GetWeeeSentOnHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.getSentOnAatfSiteDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            this.fetchWeeeSentOnAmountDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            this.addressMapper = A.Fake<IMap<AatfAddress, AatfAddressData>>();

            handler = new GetWeeeSentOnHandler(authorization, getSentOnAatfSiteDataAccess, fetchWeeeSentOnAmountDataAccess, addressMapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetWeeeSentOnHandler(authorization, getSentOnAatfSiteDataAccess, fetchWeeeSentOnAmountDataAccess, addressMapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetWeeeSentOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            await handler.HandleAsync(new GetWeeeSentOn(aatfId, returnId, null));

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(aatfId, returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void HandleAsync_GivenWeeeSentOnId_ReturnedListShouldContainOnly1Element()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();

            var sentOnList = new List<WeeeSentOn>();
            var operatorAddress = new AatfAddress("OpName", "OpAdd1", "OpAdd2", "OpTown", "OpCounty", "PostOp", A.Fake<Country>());
            var siteAddress = new AatfAddress("SiteName", "SiteAdd1", "SiteAdd2", "SiteTown", "SiteCounty", "PostSite", A.Fake<Country>());
            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, A.Fake<Aatf>(), A.Fake<Return>());
            sentOnList.Add(weeeSentOn);

            var weeeSentOnId = weeeSentOn.Id;

            var request = new GetWeeeSentOn(aatfId, returnId, weeeSentOnId);

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(aatfId, returnId)).Returns(sentOnList);

            var result = await handler.HandleAsync(request);

            result.Count.Should().Be(1);
        }

        [Fact]
        public async void HandleAsync_GivenGetSentOnAatfSiteRequest_AddressDataShouldBeMapped()
        {
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            
            var sentOnList = new List<WeeeSentOn>();
            var operatorAddress = new AatfAddress("OpName", "OpAdd1", "OpAdd2", "OpTown", "OpCounty", "PostOp", A.Fake<Country>());
            var siteAddress = new AatfAddress("SiteName", "SiteAdd1", "SiteAdd2", "SiteTown", "SiteCounty", "PostSite", A.Fake<Country>());
            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, A.Fake<Aatf>(), A.Fake<Return>());
            sentOnList.Add(weeeSentOn);

            var request = new GetWeeeSentOn(aatfId, returnId, null);

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnByReturnAndAatf(aatfId, returnId)).Returns(sentOnList);

            await handler.HandleAsync(request);

            for (var i = 0; i < sentOnList.Count; i++)
            {
                A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(sentOnList[i].OperatorAddress))).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(sentOnList[i].SiteAddress))).MustHaveHappened(Repeated.Exactly.Once);
            }
        }
    }
}
