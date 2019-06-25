namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using AutoFixture;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class RemoveReturnSchemeHandlerTests
    {
        private readonly Fixture fixture;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private RemoveReturnSchemeRequestHandler handler;

        public RemoveReturnSchemeHandlerTests()
        {
            fixture = new Fixture();
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
            var schemeIds = fixture.CreateMany<Guid>().ToList();

            RemoveReturnScheme request = new RemoveReturnScheme()
            {
                SchemeIds = schemeIds
            };

            await handler.HandleAsync(request);

            A.CallTo(() => returnSchemeDataAccess.RemoveReturnScheme(schemeIds)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
