namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchComplianceYearsWithInvoices
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.RequestHandlers.Charges.FetchComplianceYearsWithInvoices;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;
    public class FetchComplianceYearsWithInvoicesHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to the internal area cannot use
        /// the FetchComplianceYearsWithInvoicesHandler without a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            FetchComplianceYearsWithInvoicesHandler handler = new FetchComplianceYearsWithInvoicesHandler(
                authorization,
                A.Dummy<ICommonDataAccess>());

            // Act
            Func<Task<IReadOnlyList<int>>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchComplianceYearsWithInvoices>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task GetAllComplianceYearsHandler_ReturnsYearsInDescendingOrder()
        {
            // Arrange
            UKCompetentAuthority competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), "Environment Agency", "EA", new Country(Guid.NewGuid(), "UK - England"));
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.SchemeName).Returns("Test1");
            A.CallTo(() => scheme1.CompetentAuthority).Returns(competentAuthority);
            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.SchemeName).Returns("Test2");
            A.CallTo(() => scheme2.CompetentAuthority).Returns(competentAuthority);
            var scheme3 = A.Fake<Scheme>();
            A.CallTo(() => scheme3.SchemeName).Returns("Test3");
            A.CallTo(() => scheme3.CompetentAuthority).Returns(competentAuthority);

            InvoiceRun invoice = A.Fake<InvoiceRun>();

            var memberUpload1 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload1.ComplianceYear).Returns(2015);
            A.CallTo(() => memberUpload1.Scheme).Returns(scheme1);
            A.CallTo(() => memberUpload1.InvoiceRun).Returns(invoice);

            var memberUpload2 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload2.ComplianceYear).Returns(2017);
            A.CallTo(() => memberUpload2.Scheme).Returns(scheme2);
            A.CallTo(() => memberUpload2.InvoiceRun).Returns(invoice);

            var memberUpload3 = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload3.ComplianceYear).Returns(2016);
            A.CallTo(() => memberUpload3.Scheme).Returns(scheme3);
            A.CallTo(() => memberUpload3.InvoiceRun).Returns(invoice);

            ICommonDataAccess dataAccess = A.Fake<ICommonDataAccess>();
            A.CallTo(() => dataAccess.FetchInvoicedMemberUploadsAsync(competentAuthority))
                .Returns(new List<MemberUpload>()
                {
                     memberUpload1,
                    memberUpload2,
                    memberUpload3
                });
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            FetchComplianceYearsWithInvoicesHandler handler = new FetchComplianceYearsWithInvoicesHandler(authorization, dataAccess);

            Requests.Charges.FetchComplianceYearsWithInvoices request = new Requests.Charges.FetchComplianceYearsWithInvoices(CompetentAuthority.England);
            A.CallTo(() => dataAccess.FetchCompetentAuthority(CompetentAuthority.England)).Returns(competentAuthority); 
            
            // Act
            var yearsList = await handler.HandleAsync(request);
            Assert.NotNull(yearsList);
            Assert.Equal(3, yearsList.Count);
            Assert.Collection(yearsList,
                r1 => Assert.Equal("2015", r1.ToString()),
                r2 => Assert.Equal("2016", r2.ToString()),
                r3 => Assert.Equal("2017", r3.ToString()));
        }
    }
}
