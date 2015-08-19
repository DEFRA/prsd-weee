namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Core.Helpers;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using SchemeStatus = Core.Shared.SchemeStatus;

    public class SetSchemeStatusHandlerTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public SetSchemeStatusHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
        }

        [Fact]
        public async void SchemeDoesNotExist_ThrowsInvalidOperationException_AndDoesNotSaveAnyChanges()
        {
            var schemeId = Guid.NewGuid();

            A.CallTo(() => context.Schemes)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Scheme>()));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => SetSchemeStatusHandler().HandleAsync(new SetSchemeStatus(schemeId, A<SchemeStatus>._)));
        }

        [Theory]
        [InlineData(SchemeStatus.Approved)]
        [InlineData(SchemeStatus.Pending)]
        [InlineData(SchemeStatus.Rejected)]
        public async void SchemeDoesExist_SetsSchemeStatus_AndSavesChanges(SchemeStatus status)
        {
            var scheme = new Scheme(Guid.NewGuid());

            A.CallTo(() => context.Schemes)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Scheme>
                {
                    scheme
                }));

            await SetSchemeStatusHandler().HandleAsync(new SetSchemeStatus(scheme.Id, status));

            Assert.Equal(status.ToDomainEnumeration<Domain.Scheme.SchemeStatus>(), context.Schemes.Single().SchemeStatus);
            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }

        private SetSchemeStatusHandler SetSchemeStatusHandler()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowEverything().Build();

            return new SetSchemeStatusHandler(context, authorization);
        }
    }
}
