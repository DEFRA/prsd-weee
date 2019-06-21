namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchSchemesWithInvoices
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using RequestHandlers.Charges;
    using RequestHandlers.Charges.FetchSchemesWithInvoices;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Charges;
    using Weee.Tests.Core;
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
                A.Dummy<ICommonDataAccess>(),
                A.Dummy<IMap<Scheme, SchemeData>>());

            // Act
            Func<Task<IReadOnlyList<SchemeData>>> testCode = async () => await handler.HandleAsync(A.Dummy<Requests.Charges.FetchSchemesWithInvoices>());

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task GetAllInvoicedSchemesHandler_ReturnsSchemes()
        {
            // Arrange
            UKCompetentAuthority competentAuthority =
                new UKCompetentAuthority(Guid.NewGuid(), "Environment Agency", "EA", new Country(Guid.NewGuid(), "UK - England"), "test@sfwltd.co.uk", 0);
            var scheme1 = A.Fake<Scheme>();
            
            A.CallTo(() => scheme1.CompetentAuthority).Returns(competentAuthority);
            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.CompetentAuthority).Returns(competentAuthority);
            var scheme3 = A.Fake<Scheme>();
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

            var schemeData1 = A.Fake<SchemeData>();
            var schemeData2 = A.Fake<SchemeData>();
            var schemeData3 = A.Fake<SchemeData>();

            ICommonDataAccess dataAccess = A.Fake<ICommonDataAccess>();
            A.CallTo(() => dataAccess.FetchInvoicedMemberUploadsAsync(competentAuthority))
                .Returns(new List<MemberUpload>()
                {
                     memberUpload1,
                    memberUpload2,
                    memberUpload3
                });

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            var schemeMap = A.Fake<IMap<Scheme, SchemeData>>();

            A.CallTo(() => schemeMap.Map(memberUpload1.Scheme)).Returns(schemeData1);
            A.CallTo(() => schemeMap.Map(memberUpload2.Scheme)).Returns(schemeData2);
            A.CallTo(() => schemeMap.Map(memberUpload3.Scheme)).Returns(schemeData3);

            FetchSchemesWithInvoicesHandler handler = new FetchSchemesWithInvoicesHandler(
               authorization,
               dataAccess,
                schemeMap);

            FetchSchemesWithInvoices request = new FetchSchemesWithInvoices(CompetentAuthority.England);
            A.CallTo(() => dataAccess.FetchCompetentAuthority(CompetentAuthority.England)).Returns(competentAuthority);
            // Act
            var schemesList = await handler.HandleAsync(request);

            //Assert
            Assert.NotNull(schemesList);
            Assert.Equal(3, schemesList.Count);
            Assert.Collection(schemesList,
                r1 => Assert.Equal(r1, schemeData1),
                r2 => Assert.Equal(r2, schemeData2),
                r3 => Assert.Equal(r3, schemeData3));
        }
    }
}
