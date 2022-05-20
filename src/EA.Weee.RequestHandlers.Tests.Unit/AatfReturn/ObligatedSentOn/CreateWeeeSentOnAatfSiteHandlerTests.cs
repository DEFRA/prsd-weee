namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.Domain.AatfReturn;
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

    public class CreateWeeeSentOnAatfSiteHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly CreateWeeeSentOnAatfSiteHandler handler;

        public CreateWeeeSentOnAatfSiteHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.getSentOnAatfSiteDataAccess = A.Fake<IWeeeSentOnDataAccess>();

            handler = new CreateWeeeSentOnAatfSiteHandler(authorization, getSentOnAatfSiteDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new CreateWeeeSentOnAatfSiteHandler(authorization, getSentOnAatfSiteDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CreateWeeeSentOnAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_CreateWeeeSentOnAatfSiteRequest_DataSubmitIsCalled()
        {
            var aatf = A.Fake<Aatf>();
            var weeeSentOnId = Guid.NewGuid();
            var siteAddress = A.Fake<AatfAddress>();
            var aatfReturn = ReturnHelper.GetReturn();

            var weeeSentOn = new WeeeSentOn(aatf.Id, aatfReturn.Id, siteAddress.Id);

            var createWeeeSentOnAatf = A.Fake<CreateWeeeSentOnAatfSite>();

            var requestHandler = new CreateWeeeSentOnAatfSiteHandler(authorization, getSentOnAatfSiteDataAccess);

            await requestHandler.HandleAsync(createWeeeSentOnAatf);

            A.CallTo(() => getSentOnAatfSiteDataAccess.Submit(A<WeeeSentOn>.That.IsSameAs(weeeSentOn)));
        }
    }
}
