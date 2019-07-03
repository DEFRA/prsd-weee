namespace EA.Weee.RequestHandlers.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddReturnSchemeHandlerTests
    {
        private readonly Fixture fixture;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private AddReturnSchemeHandler handler;

        public AddReturnSchemeHandlerTests()
        {
            fixture = new Fixture();
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            returnDataAccess = A.Fake<IReturnDataAccess>();

            handler = new AddReturnSchemeHandler(weeeAuthorization, returnSchemeDataAccess, returnDataAccess, schemeDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new AddReturnSchemeHandler(authorization, A.Dummy<IReturnSchemeDataAccess>(), A.Dummy<IReturnDataAccess>(), A.Dummy<ISchemeDataAccess>());

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddReturnScheme>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_SchemeShouldBeRetrieved()
        {
            var schemeIds = fixture.CreateMany<Guid>().ToList();

            var request = new AddReturnScheme { SchemeIds = schemeIds };

            await handler.HandleAsync(request);

            foreach (var schemeId in schemeIds)
            {
                A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(schemeId)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var request = new AddReturnScheme { ReturnId = returnId, SchemeIds = new List<Guid>() };

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.GetById(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_DataAccessSubmitsIsCalled()
        {
            var schemeId = Guid.NewGuid();
            var request = new AddReturnScheme { ReturnId = Guid.NewGuid(), SchemeIds = new List<Guid> { schemeId } };

            var @return = A.Dummy<Return>();
            var scheme = A.Fake<Domain.Scheme.Scheme>();

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(schemeId)).Returns(scheme);

            A.CallTo(() => returnDataAccess.GetById(request.ReturnId)).Returns(@return);

            await handler.HandleAsync(request);
            
            A.CallTo(() => returnSchemeDataAccess.Submit(A<List<ReturnScheme>>.That.Matches(c => c.First().ReturnId == @return.Id && c.First().SchemeId == scheme.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_ReturnSchemeIdShouldBeReturned()
        {
            var request = new AddReturnScheme { ReturnId = Guid.NewGuid(), SchemeIds = fixture.CreateMany<Guid>().ToList() };
            var id = fixture.CreateMany<Guid>().ToList();
            
            A.CallTo(() => returnSchemeDataAccess.Submit(A<List<ReturnScheme>>._)).Returns(id);

            var result = await handler.HandleAsync(request);

            result.Should().Equal(id);
        }
    }
}