namespace EA.Weee.RequestHandlers.Tests.Unit
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetSchemeRequestHandlerTests
    {
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly RequestHandlers.Scheme.IGetSchemesDataAccess getSchemesDataAccess;
        private readonly IReturnDataAccess returnDataAccess; 
        private readonly ISchemeDataAccess schemeDataAccess;
        private GetSchemeRequestHandler handler;
        private readonly IMapper mapper;
   
        public GetSchemeRequestHandlerTests()
        {
            var weeeAuthorization = A.Fake<IWeeeAuthorization>();
            returnSchemeDataAccess = A.Fake<IReturnSchemeDataAccess>();
            getSchemesDataAccess = A.Fake<RequestHandlers.Scheme.IGetSchemesDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            returnDataAccess = A.Fake<IReturnDataAccess>();
            mapper = A.Fake<IMapper>();
            handler = new GetSchemeRequestHandler(weeeAuthorization, returnSchemeDataAccess, getSchemesDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            
            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetReturnScheme>());
            
            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetReturnSchemeRequest_SchemeShouldBeRetrieved()
        {
            var returnId = Guid.NewGuid();
            var request = new GetReturnScheme(returnId);

            await handler.HandleAsync(request);

            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(returnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenGetReturnSchemeRequest_ReturnSchemeDataAccessIsCalled()
        {
            var returnId = Guid.NewGuid();
            List<Guid> schemeIdList = new List<Guid>();

            var request = new GetReturnScheme(returnId);

            Domain.Scheme.Scheme scheme = A.Fake<Domain.Scheme.Scheme>();

            A.CallTo(() => returnSchemeDataAccess.GetSelectedSchemesByReturnId(request.ReturnId)).Returns(schemeIdList);
            A.CallTo(() => getSchemesDataAccess.GetSchemeBasedOnId(A<Guid>._)).Returns(scheme);

            var mapper = A.Fake<IMapper>();
            SchemeData schemeData = A.Fake<SchemeData>();
            A.CallTo(() => mapper.Map<Domain.Scheme.Scheme, SchemeData>(scheme))
                .Returns(schemeData);

            var result = await handler.HandleAsync(request);
        }
    }
}