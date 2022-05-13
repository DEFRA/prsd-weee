namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class RemoveWeeeSentOnHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedWeeeDataAccess;
        private readonly RemoveWeeeSentOnHandler handler;

        public RemoveWeeeSentOnHandlerTests()
        {
            this.context = A.Fake<WeeeContext>();
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.sentOnDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            this.genericDataAccess = A.Fake<IGenericDataAccess>();
            this.obligatedWeeeDataAccess = A.Fake<IFetchObligatedWeeeForReturnDataAccess>();

            handler = new RemoveWeeeSentOnHandler(context, authorization, sentOnDataAccess, genericDataAccess, obligatedWeeeDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new RemoveWeeeSentOnHandler(context, authorization, sentOnDataAccess, genericDataAccess, obligatedWeeeDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<RemoveWeeeSentOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var weeeSentOnId = Guid.NewGuid();
            var weeeSentOn = new WeeeSentOn();
            var weeeSentOnAmount = new List<WeeeSentOnAmount>();

            A.CallTo(() => genericDataAccess.GetById<WeeeSentOn>(weeeSentOnId)).Returns(weeeSentOn);
            A.CallTo(() => obligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturn(weeeSentOnId)).Returns(weeeSentOnAmount);

            await handler.HandleAsync(new RemoveWeeeSentOn(weeeSentOnId));

            A.CallTo(() => genericDataAccess.Remove(weeeSentOn)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => genericDataAccess.RemoveMany(weeeSentOnAmount)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_ProvideNonExistentWeeeSentOnId_ReturnsFalse()
        {
            WeeeSentOn returnData = null;
            var request = new RemoveWeeeSentOn(A.Dummy<Guid>());

            A.CallTo(() => genericDataAccess.GetById<WeeeSentOn>(A.Dummy<Guid>())).Returns(returnData);

            bool result = await handler.HandleAsync(request);

            Assert.False(result);
        }
    }
}
