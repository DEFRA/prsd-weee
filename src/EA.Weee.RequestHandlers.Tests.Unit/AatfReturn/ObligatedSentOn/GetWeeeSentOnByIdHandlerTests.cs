namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
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
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetWeeeSentOnByIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly GetWeeeSentOnByIdHandler handler;
        private readonly WeeeContext context;

        public GetWeeeSentOnByIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.getSentOnAatfSiteDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            this.fetchWeeeSentOnAmountDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            this.addressMapper = A.Fake<IMap<AatfAddress, AatfAddressData>>();
            this.context = A.Fake<WeeeContext>();

            handler = new GetWeeeSentOnByIdHandler(authorization, getSentOnAatfSiteDataAccess, fetchWeeeSentOnAmountDataAccess, addressMapper, context);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new GetWeeeSentOnHandler(authorization, getSentOnAatfSiteDataAccess, fetchWeeeSentOnAmountDataAccess, addressMapper, context);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetWeeeSentOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var weeeSentOnId = Guid.NewGuid();

            await handler.HandleAsync(new GetWeeeSentOnById(weeeSentOnId));

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnById(weeeSentOnId)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenWeeeSentOnByIdRequest_AddressDataShouldBeMapped()
        {
            var operatorAddress = new AatfAddress("OpName", "OpAdd1", "OpAdd2", "OpTown", "OpCounty", "PostOp", A.Fake<Country>());
            var siteAddress = new AatfAddress("SiteName", "SiteAdd1", "SiteAdd2", "SiteTown", "SiteCounty", "PostSite", A.Fake<Country>());
            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, A.Fake<Aatf>(), A.Fake<Return>());
            var weeeSentOnId = weeeSentOn.Id;

            var request = new GetWeeeSentOnById(weeeSentOnId);

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnById(weeeSentOnId)).Returns(weeeSentOn);

            await handler.HandleAsync(request);

            A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(weeeSentOn.OperatorAddress))).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(weeeSentOn.SiteAddress))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_ProvideNonExistantWeeeSentOnId_ReturnsNull()
        {
            WeeeSentOn returnData = null;
            var request = new GetWeeeSentOnById(A.Dummy<Guid>());

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnById(A.Dummy<Guid>())).Returns(returnData);

            WeeeSentOnData result = await handler.HandleAsync(request);

            Assert.Null(result);
        }
    }
}
