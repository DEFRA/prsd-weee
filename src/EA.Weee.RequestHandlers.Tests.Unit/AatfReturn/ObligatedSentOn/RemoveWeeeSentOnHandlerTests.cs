namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
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

    public class RemoveWeeeSentOnHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly RemoveWeeeSentOnHandler handler;

        public RemoveWeeeSentOnHandlerTests()
        {
            this.context = A.Fake<WeeeContext>();
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.sentOnDataAccess = A.Fake<IWeeeSentOnDataAccess>();
            this.genericDataAccess = A.Fake<IGenericDataAccess>();

            handler = new RemoveWeeeSentOnHandler(context, authorization, sentOnDataAccess, genericDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            var handler = new RemoveWeeeSentOnHandler(context, authorization, sentOnDataAccess, genericDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<RemoveWeeeSentOn>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSentOnAatfSiteRequest_DataAccessIsCalled()
        {
            var weeeSentOnId = Guid.NewGuid();
            var weeeSentOn = new WeeeSentOn();

            A.CallTo(() => genericDataAccess.GetById<WeeeSentOn>(weeeSentOnId)).Returns(weeeSentOn);

            await handler.HandleAsync(new RemoveWeeeSentOn(weeeSentOnId));

            A.CallTo(() => sentOnDataAccess.UpdateWeeeSentOnAsRemoved(weeeSentOn)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
