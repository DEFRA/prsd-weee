namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedReused
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.AatfReturn.Specification;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class RemoveAatfSiteHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly RemoveAatfSiteHandler handler;

        public RemoveAatfSiteHandlerTests()
        {
            this.context = A.Fake<WeeeContext>();
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new RemoveAatfSiteHandler(context, authorization, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new RemoveAatfSiteHandler(context, authorization, genericDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<RemoveAatfSite>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var siteAddressId = Guid.NewGuid();
            var weeeReusedSiteId = Guid.NewGuid();
            var siteAddress = new AatfAddress();
            var weeeReusedSite = new WeeeReusedSite();
            var weeeReusedSiteReturned = new WeeeReusedSite();

            A.CallTo(() => genericDataAccess.GetById<AatfAddress>(siteAddressId)).Returns(siteAddress);
            A.CallTo(() => genericDataAccess.GetSingleByExpression<WeeeReusedSite>(A<WeeeReusedByAddressIdSpecification>.That.Matches(w => w.SiteId == siteAddressId))).Returns(weeeReusedSite);
            A.CallTo(() => genericDataAccess.GetById<WeeeReusedSite>(weeeReusedSite.Id)).Returns(weeeReusedSiteReturned);

            await handler.HandleAsync(new RemoveAatfSite(siteAddressId));

            A.CallTo(() => genericDataAccess.Remove(weeeReusedSiteReturned)).MustHaveHappened(1, Times.Exactly)
                .Then(A.CallTo(() => genericDataAccess.Remove(siteAddress)).MustHaveHappened(1, Times.Exactly))
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }
    }
}
