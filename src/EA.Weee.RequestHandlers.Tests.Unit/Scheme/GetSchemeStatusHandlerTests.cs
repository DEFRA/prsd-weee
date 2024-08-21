namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme;
    using RequestHandlers.Security;
    using Requests.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class GetSchemeStatusHandlerTests
    {
        private readonly DbContextHelper contextHelper;
        private readonly WeeeContext context;

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        private readonly IMap<Domain.Scheme.SchemeStatus, SchemeStatus> mapper;

        public GetSchemeStatusHandlerTests()
        {
            contextHelper = new DbContextHelper();
            context = A.Fake<WeeeContext>();
            mapper = A.Fake<IMap<Domain.Scheme.SchemeStatus, SchemeStatus>>();
        }

        [Fact]
        public async Task NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new GetSchemeStatusHandler(context, denyingAuthorization, mapper);

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await handler.HandleAsync(new GetSchemeStatus(Guid.NewGuid())));
        }

        [Fact]
        public async Task HandleAsync_SchemeDoesNotExist_ThrowsInvalidOperationException()
        {
            var schemeId = Guid.NewGuid();

            A.CallTo(() => context.Schemes)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Scheme>()));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => GetSchemeStatusHandler().HandleAsync(new GetSchemeStatus(schemeId)));
        }

        [Fact]
        public async Task HandleAsync_SchemeExists_MapsStatus()
        {
            var schemeId = Guid.NewGuid();

            var scheme = new Scheme(schemeId);

            A.CallTo(() => context.Schemes)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Scheme>
                {
                    scheme
                }));

            await GetSchemeStatusHandler().HandleAsync(new GetSchemeStatus(schemeId));

            A.CallTo(() => mapper.Map(A<Domain.Scheme.SchemeStatus>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        private GetSchemeStatusHandler GetSchemeStatusHandler()
        {
            return new GetSchemeStatusHandler(context, permissiveAuthorization, mapper);
        }
    }
}
