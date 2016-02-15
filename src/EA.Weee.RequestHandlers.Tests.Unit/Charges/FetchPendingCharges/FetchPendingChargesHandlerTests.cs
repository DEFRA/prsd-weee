namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchPendingCharges
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Charges;
    using Domain;
    using Domain.Obligation;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Charges;
    using RequestHandlers.Charges.FetchPendingCharges;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchPendingChargesHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to the internal area cannot use
        /// the FetchPendingChargesHandler without a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            FetchPendingChargesHandler handler = new FetchPendingChargesHandler(
                authorization,
                A.Dummy<ICommonDataAccess>());

            // Act
            Func<Task<ManagePendingCharges>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchPendingCharges>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_ReturnsTrueForCanUserIssueCharges_WhenCurrentUserIsInternalAdmin()
        {
            // Arrange
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .AllowRole(Roles.InternalAdmin)
                .Build();

            var handler = new FetchPendingChargesHandler(
                weeeAuthorization,
                A.Dummy<ICommonDataAccess>());
            
            // Act
            var result = await handler.HandleAsync(A.Dummy<Requests.Charges.FetchPendingCharges>());

            // Assert
            Assert.True(result.CanUserIssueCharges);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFalseForCanUserIssueCharges_WhenCurrentUserIsNotInternalAdmin()
        {
            // Arrange
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new FetchPendingChargesHandler(
                 weeeAuthorization,
                 A.Dummy<ICommonDataAccess>());

            // Act
            var result = await handler.HandleAsync(A.Dummy<Requests.Charges.FetchPendingCharges>());

            // Assert
            Assert.False(result.CanUserIssueCharges);
        }

        /// <summary>
        /// This test ensures that the member uploads returned from the data access
        /// will be grouped by scheme and compliance year. In this test, four member uploads
        /// are returned with two having the same scheme (scheme1) and compliance year (2017)
        /// which should result in 3 charges; the first of which should have a total of
        /// 10 + 20 = 30.
        /// The ordering of the results should be handled by the data access, but the
        /// handler should not re-order the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_Always_GroupsMemberUploadsBySchemeAndComplianceYear()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Scheme scheme1 = new Scheme(A.Dummy<Guid>());
            scheme1.UpdateScheme("AAAA", "WEE/AA1111AA/SCH", null, ObligationType.Both, A.Dummy<Guid>());

            Scheme scheme2 = new Scheme(A.Dummy<Guid>());
            scheme2.UpdateScheme("BBBB", "WEE/BB2222BB/SCH", null, ObligationType.Both, A.Dummy<Guid>());

            List<MemberUpload> memberUploads = new List<MemberUpload>();

            MemberUpload memberUpload1 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                10,
                2017,
                scheme1,
                A.Dummy<string>());

            MemberUpload memberUpload2 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                20,
                2017,
                scheme1,
                A.Dummy<string>());

            MemberUpload memberUpload3 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                40,
                2016,
                scheme1,
                A.Dummy<string>());

            MemberUpload memberUpload4 = new MemberUpload(
                A.Dummy<Guid>(),
                A.Dummy<string>(),
                A.Dummy<List<MemberUploadError>>(),
                80,
                2017,
                scheme2,
                A.Dummy<string>());

            memberUploads.Add(memberUpload1);
            memberUploads.Add(memberUpload2);
            memberUploads.Add(memberUpload3);
            memberUploads.Add(memberUpload4);

            ICommonDataAccess dataAccess = A.Fake<ICommonDataAccess>();
            A.CallTo(() => dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(A<UKCompetentAuthority>._))
                .Returns(memberUploads);

            FetchPendingChargesHandler handler = new FetchPendingChargesHandler(
               authorization,
               dataAccess);

            // Act
            var results = await handler.HandleAsync(A.Dummy<Requests.Charges.FetchPendingCharges>());

            // Assert
            Assert.NotNull(results);
            Assert.Equal(3, results.PendingCharges.Count);

            Assert.Collection(results.PendingCharges,
                r1 =>
                {
                    Assert.Equal("AAAA", r1.SchemeName);
                    Assert.Equal(2017, r1.ComplianceYear);
                    Assert.Equal(30, r1.TotalGBP);
                },
                r2 =>
                {
                    Assert.Equal("AAAA", r2.SchemeName);
                    Assert.Equal(2016, r2.ComplianceYear);
                    Assert.Equal(40, r2.TotalGBP);
                },
                r3 =>
                {
                    Assert.Equal("BBBB", r3.SchemeName);
                    Assert.Equal(2017, r3.ComplianceYear);
                    Assert.Equal(80, r3.TotalGBP);
                });
        }
    }
}
