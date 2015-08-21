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
    using System.Security;
    using System.Threading.Tasks;
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

        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to set a scheme's status.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("Authorization", "Internal")]
        public async Task SetSchemeStatusHandler_WithUnauthorizedUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithNoRights();

            SetSchemeStatusHandler handler = new SetSchemeStatusHandler(context, authorization);

            Guid schemeId = new Guid("3C367528-AE93-427F-A4C5-E23F0D317633");
            SetSchemeStatus request = new SetSchemeStatus(schemeId, SchemeStatus.Approved);

            // Act
            Func<Task<Guid>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
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
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            return new SetSchemeStatusHandler(context, authorization);
        }
    }
}
