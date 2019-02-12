namespace EA.Weee.RequestHandlers.Tests.Unit
{
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
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

    public class AddReturnSchemeHandlerTests
    {
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private AddReturnSchemeHandler handler;

        public AddReturnSchemeHandlerTests()
        {
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
            var schemeId = Guid.NewGuid();

            var request = new AddReturnScheme { SchemeId = schemeId };

            await handler.HandleAsync(request);

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(schemeId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_ReturnShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();

            var request = new AddReturnScheme { ReturnId = returnId };

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.GetById(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenAddReturnSchemeRequest_DataAccessSubmitsIsCalled()
        {
            var request = new AddReturnScheme { ReturnId = Guid.NewGuid(), SchemeId = Guid.NewGuid() };

            var @return = A.Dummy<Return>();
            var scheme = A.Fake<Domain.Scheme.Scheme>();

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(request.SchemeId)).Returns(scheme);

            A.CallTo(() => returnDataAccess.GetById(request.ReturnId)).Returns(@return);

            await handler.HandleAsync(request);
            
            A.CallTo(() => returnSchemeDataAccess.Submit(A<ReturnScheme>.That.Matches(c => c.ReturnId == @return.Id && c.SchemeId == scheme.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}