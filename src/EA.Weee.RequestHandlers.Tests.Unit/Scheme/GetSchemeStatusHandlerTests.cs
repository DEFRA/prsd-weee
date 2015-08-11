namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Helpers;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using Xunit;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class GetSchemeStatusHandlerTests
    {
        private readonly DbContextHelper contextHelper;
        private readonly WeeeContext context;
        private readonly IMap<Domain.Scheme.SchemeStatus, SchemeStatus> mapper;

        public GetSchemeStatusHandlerTests()
        {
            contextHelper = new DbContextHelper();
            context = A.Fake<WeeeContext>();
            mapper = A.Fake<IMap<Domain.Scheme.SchemeStatus, SchemeStatus>>();
        }

        [Fact]
        public async void HandleAsync_SchemeDoesNotExist_ThrowsInvalidOperationException()
        {
            var schemeId = Guid.NewGuid();

            A.CallTo(() => context.Schemes)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<Scheme>()));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => GetSchemeStatusHandler().HandleAsync(new GetSchemeStatus(schemeId)));
        }

        [Fact]
        public async void HandleAsync_SchemeExists_MapsStatus()
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
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private GetSchemeStatusHandler GetSchemeStatusHandler()
        {
            return new GetSchemeStatusHandler(context, mapper);
        }
    }
}
