namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using Core.Helpers;
    using Core.Shared;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using ObligationType = Domain.ObligationType;

    public class UpdateSchemeInformationHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        /// <summary>
        /// This test ensures that a non-internal user cannot execute requests to update
        /// a scheme's information.
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("Authorization", "Internal")]
        public async Task UpdateSchemeInformationHandler_WithUnauthorizedUser_ThrowsSecurityException()
        {
            // Arrange
            WeeeContext context = A.Fake<WeeeContext>();
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithNoRights();

            UpdateSchemeInformationHandler handler = new UpdateSchemeInformationHandler(context, authorization);

            UpdateSchemeInformation request = new UpdateSchemeInformation(
                new Guid("3C367528-AE93-427F-A4C5-E23F0D317633"),
                "New scheme name",
                "New approval name",
                "New IBIS customer reference",
                Core.Shared.ObligationType.B2B,
                new Guid("{291F9DE0-2600-481C-9C66-5ACB4033668C}"),
                Core.Shared.SchemeStatus.Approved);

            // Act
            Func<Task<Guid>> action = () => handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task UpdateSchemeInformationHandler_UpdateSchemeKeyInformation_ReturnsUpdatedScheme()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            var handler = new UpdateSchemeInformationHandler(context, authorization);

            const string schemeName = "Scheme name";
            const string approvalNumber = "WEE/AB8888CD/SCH";
            const string ibisCustomerReference = "Any value";
            var obligationType = EA.Weee.Core.Shared.ObligationType.B2B;
            var status = Core.Shared.SchemeStatus.Approved;
            var competentAuthorityId = Guid.NewGuid();

            await
                handler.HandleAsync(new UpdateSchemeInformation(schemes.First().Id, schemeName, approvalNumber, ibisCustomerReference, obligationType, competentAuthorityId, status));

            var schemeInfo = schemes.FirstOrDefault();

            Assert.NotNull(schemeInfo);
            Assert.Equal(schemeInfo.ObligationType, Domain.ObligationType.B2B);
            Assert.Equal(schemeInfo.ApprovalNumber, approvalNumber);
            Assert.Equal(schemeInfo.SchemeName, schemeName);
            Assert.Equal(schemeInfo.IbisCustomerReference, ibisCustomerReference);
            Assert.Equal(schemeInfo.CompetentAuthorityId, competentAuthorityId);
            Assert.Equal(status.ToDomainEnumeration<Domain.Scheme.SchemeStatus>(), schemeInfo.SchemeStatus);
        }

        [Fact]
        public async Task UpdateSchemeInformationHandler_UpdateSchemeKeyInformation_ApprovalNumberAlreadyExists_ThrowException()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            var handler = new UpdateSchemeInformationHandler(context, authorization);

            const string schemeName = "Scheme name";
            const string approvalNumber = "WEE/AB1234CD/SCH";
            const string ibisCustomerReference = "Any value";
            var obligationType = EA.Weee.Core.Shared.ObligationType.B2B;
            var status = Core.Shared.SchemeStatus.Approved;
            var competentAuthorityId = Guid.NewGuid();

            await
                Assert.ThrowsAsync<Exception>(
                    async () => await handler.HandleAsync(new UpdateSchemeInformation(schemes.First().Id, schemeName, approvalNumber,
                    ibisCustomerReference, obligationType, competentAuthorityId, status)));
        }

        private DbSet<Scheme> MakeScheme()
        {
            return helper.GetAsyncEnabledDbSet(new List<Scheme>
            {
                new Scheme(Guid.NewGuid()),
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
