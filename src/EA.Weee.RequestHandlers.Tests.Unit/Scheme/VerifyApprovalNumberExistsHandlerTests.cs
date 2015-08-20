namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Xunit;
    using ObligationType = Domain.ObligationType;

    public class VerifyApprovalNumberExistsHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        [Fact]
        public async Task VerifyApprovalNumberExistsHandler_ApprovalNumberNotExists_ReturnsFalse()
        {
            // Arrange
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowEverything().Build();

            var handler = new VerifyApprovalNumberExistsHandler(context, authorization);

            const string approvalNumber = "WEE/AB4444CD/SCH";

            // Act
            var approvalNumberExists = await handler.HandleAsync(new VerifyApprovalNumberExists(approvalNumber));

            // Assert
            Assert.False(approvalNumberExists);
        }

        [Fact]
        public async Task VerifyApprovalNumberExistsHandler_ApprovalNumberExists_ReturnsTrue()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowEverything().Build();

            var handler = new VerifyApprovalNumberExistsHandler(context, authorization);

            const string approvalNumber = "WEE/AB1234CD/SCH";

            var approvalNumberExists = await handler.HandleAsync(new VerifyApprovalNumberExists(approvalNumber));

            Assert.True(approvalNumberExists);
        }

        private DbSet<Scheme> MakeScheme()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                CreateScheme()
            });
        }

        private static Scheme CreateScheme()
        {
            var scheme = new Scheme(Guid.NewGuid());
            scheme.UpdateScheme("Any value", "WEE/AB1234CD/SCH", "Any value", ObligationType.B2B, Guid.NewGuid());
            return scheme;
        }
    }
}
