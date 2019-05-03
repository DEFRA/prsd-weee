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

    public class GetWeeeSentOnByIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess fetchWeeeSentOnAmountDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> addressMapper;
        private readonly GetWeeeSentOnByIdHandler handler;

        public GetWeeeSentOnByIdHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.getSentOnAatfSiteDataAccess = A.Fake<ISentOnAatfSiteDataAccess>();
            this.fetchWeeeSentOnAmountDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();
            this.addressMapper = A.Fake<IMap<AatfAddress, AatfAddressData>>();

            handler = new GetWeeeSentOnByIdHandler(authorization, getSentOnAatfSiteDataAccess, fetchWeeeSentOnAmountDataAccess, addressMapper);
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
            var weeeSentOnId = Guid.NewGuid();

            await handler.HandleAsync(new GetWeeeSentOnById(weeeSentOnId));

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnById(weeeSentOnId)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public async void HandleAsync_GivenWeeeSentOnByIdRequest_AddressDataShouldBeMapped()
        {
            var operatorAddress = new AatfAddress("OpName", "OpAdd1", "OpAdd2", "OpTown", "OpCounty", "PostOp", A.Fake<Country>());
            var siteAddress = new AatfAddress("SiteName", "SiteAdd1", "SiteAdd2", "SiteTown", "SiteCounty", "PostSite", A.Fake<Country>());
            var weeeSentOn = new WeeeSentOn(operatorAddress, siteAddress, A.Fake<Aatf>(), A.Fake<Return>());
            var weeeSentOnId = weeeSentOn.Id;

            var request = new GetWeeeSentOnById(weeeSentOnId);

            A.CallTo(() => getSentOnAatfSiteDataAccess.GetWeeeSentOnById(weeeSentOnId)).Returns(weeeSentOn);

            await handler.HandleAsync(request);

            A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(weeeSentOn.OperatorAddress))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => addressMapper.Map(A<AatfAddress>.That.IsSameAs(weeeSentOn.SiteAddress))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
