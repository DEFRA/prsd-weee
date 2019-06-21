namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Admin.GetSchemes;
    using EA.Weee.RequestHandlers.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Scheme;
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
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;

    public class GetSchemesExternalHandlerTests
    {
        private readonly RequestHandlers.Scheme.IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly IWeeeAuthorization authorization;
        private GetSchemesExternalHandler handler;
        private readonly WeeeContext context;

        public GetSchemesExternalHandlerTests()
        {
            dataAccess = A.Fake<RequestHandlers.Scheme.IGetSchemesDataAccess>();
            schemeMap = A.Fake<IMap<Scheme, SchemeData>>();
            authorization = A.Fake<IWeeeAuthorization>();
            context = A.Fake<WeeeContext>();

            handler = new GetSchemesExternalHandler(dataAccess, schemeMap, authorization);
        }

        [Fact]
        public async Task HandleAsync_NoExternalAccess_ThrowsSecurityException()
        {
            var authorizationDeny = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetSchemesExternalHandler(A.Fake<RequestHandlers.Scheme.IGetSchemesDataAccess>(), A.Dummy<IMap<Scheme, SchemeData>>(), authorizationDeny);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<GetSchemesExternal>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenGetSchemeExternalRequest_ListOfSchemesShouldBeReturned()
        {
            var request = new GetSchemesExternal();

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.GetCompleteSchemes()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
