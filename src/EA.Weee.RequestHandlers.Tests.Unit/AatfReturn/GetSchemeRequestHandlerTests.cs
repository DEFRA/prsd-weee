namespace EA.Weee.RequestHandlers.Tests.Unit
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.DataAccess.DataAccess;
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

    public class GetSchemeRequestHandlerTests
    {
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private GetSchemeRequestHandler handler;
        private readonly IMapper mapper;
   
        public GetSchemeRequestHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            
            handler = new GetSchemeRequestHandler(authorization, A.Dummy<IReturnSchemeDataAccess>(), A.Dummy<RequestHandlers.Scheme.IGetSchemesDataAccess>(), mapper);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturnScheme>());
            
            await action.Should().ThrowAsync<SecurityException>();
        }

    }
}