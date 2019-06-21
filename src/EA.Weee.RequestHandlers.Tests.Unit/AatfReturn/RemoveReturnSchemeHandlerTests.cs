namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class RemoveReturnSchemeHandlerTests
    {
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private RemoveReturnSchemeRequestHandler handler;

        public RemoveReturnSchemeHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();

            handler = new RemoveReturnSchemeRequestHandler(weeeAuthorization, returnSchemeDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new RemoveReturnSchemeRequestHandler(authorization, A.Dummy<IReturnSchemeDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<RemoveReturnScheme>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenSchemeId_ReturnSchemeDeleted()
        {
            var schemeId = Guid.NewGuid();

            RemoveReturnScheme request = new RemoveReturnScheme()
            {
                SchemeId = schemeId
            };

            await handler.HandleAsync(request);

            A.CallTo(() => returnSchemeDataAccess.RemoveReturnScheme(schemeId)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
