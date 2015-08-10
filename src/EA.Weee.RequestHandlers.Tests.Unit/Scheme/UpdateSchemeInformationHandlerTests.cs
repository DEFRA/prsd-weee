namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme.MemberUploadTesting;
    using DataAccess;
    using Domain.Scheme;
    using FakeItEasy;
    using Helpers;
    using RequestHandlers.Scheme;
    using Requests.Scheme;
    using Xunit;

    public class UpdateSchemeInformationHandlerTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();

        [Fact]
        public async Task UpdateSchemeInformationHandler_UpdateSchemeKeyInformation_ReturnsUpdatedScheme()
        {
            var schemes = MakeScheme();

            var context = A.Fake<WeeeContext>();

            A.CallTo(() => context.Schemes).Returns(schemes);

            var handler = new UpdateSchemeInformationHandler(context);

            const string schemeName = "WEE/AB1234CD/SCH";
            const string approvalNumber = "Approval number";
            const string ibisCustomerReference = "Any value";
            var obligationType = ObligationType.B2B;
            var competentAuthorityId = Guid.NewGuid();

            await
                handler.HandleAsync(new UpdateSchemeInformation(schemes.FirstOrDefault().Id, schemeName, approvalNumber, ibisCustomerReference, obligationType, competentAuthorityId));

            var schemeInfo = schemes.FirstOrDefault();

            Assert.NotNull(schemeInfo);
            Assert.Equal(schemeInfo.ObligationType, Domain.ObligationType.B2B);
            Assert.Equal(schemeInfo.ApprovalNumber, approvalNumber);
            Assert.Equal(schemeInfo.SchemeName, schemeName);
            Assert.Equal(schemeInfo.IbisCustomerReference, ibisCustomerReference);
            Assert.Equal(schemeInfo.CompetentAuthorityId, competentAuthorityId);
        }

        private DbSet<Scheme> MakeScheme()
        {
            return helper.GetAsyncEnabledDbSet(new[]
            {
                new Scheme(Guid.NewGuid())
            });
        }
    }
}
