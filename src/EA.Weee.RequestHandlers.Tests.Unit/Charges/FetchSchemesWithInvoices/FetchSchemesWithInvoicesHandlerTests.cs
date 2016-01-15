namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchSchmesWithInvoices
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.RequestHandlers.Charges.FetchSchemesWithInvoices;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Requests.Charges;
    using Xunit;

    public class FetchSchemesWithInvoicesHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to the internal area cannot use
        /// the FetchSchemesWithInvoicesHandler without a SecurityException being thrown.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNoInternalAccess_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            FetchSchemesWithInvoicesHandler handler = new FetchSchemesWithInvoicesHandler(
                authorization,
                A.Dummy<ICommonDataAccess>());

            // Act
            Func<Task<IReadOnlyList<string>>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchSchemesWithInvoices>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task GetAllInvoicedSchemesHandler_ReturnsSchemes()
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
            FetchSchemesWithInvoicesHandler handler = new FetchSchemesWithInvoicesHandler(
               authorization,
               dataAccess);

            FetchSchemesWithInvoices request = new FetchSchemesWithInvoices(CompetentAuthority.England);
            A.CallTo(() => dataAccess.FetchCompetentAuthority(CompetentAuthority.England)).Returns(competentAuthority);
            // Act
            var schemesList = await handler.HandleAsync(request);

            //Assert
            Assert.NotNull(schemesList);
            Assert.Equal(3, schemesList.Count);
            Assert.Collection(schemesList,
                r1 => Assert.Equal("Test1", r1.ToString()),
                r2 => Assert.Equal("Test2", r2.ToString()),
                r3 => Assert.Equal("Test3", r3.ToString()));
        }   
    }
}
