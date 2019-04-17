namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchInvoiceRunCsv
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Domain.Charges;
    using FakeItEasy;
    using RequestHandlers.Charges;
    using RequestHandlers.Charges.FetchInvoiceRunCsv;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;
    using FetchInvoiceRunCsv = Requests.Charges.FetchInvoiceRunCsv;

    public class FetchInvoiceRunCsvHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Dummy<WeeeContext>();
            var dataAccess = A.Dummy<ICommonDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchInvoiceRunCsvHandler(authorization, context, dataAccess, csvWriterFactory);

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<FetchInvoiceRunCsv>()));
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Dummy<WeeeContext>();
            var dataAccess = A.Dummy<ICommonDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchInvoiceRunCsvHandler(authorization, context, dataAccess, csvWriterFactory);

            var data = await handler.HandleAsync(A.Dummy<FetchInvoiceRunCsv>());

            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileNameWithInvoiceIssuedDateAndAA()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Dummy<WeeeContext>();
            var dataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchInvoiceRunCsvHandler(authorization, context, dataAccess, csvWriterFactory);

            var invoiceRun = A.Fake<InvoiceRun>();
            A.CallTo(() => invoiceRun.CompetentAuthority)
                .Returns(new UKCompetentAuthority(Guid.NewGuid(), "Wales", "NRW", A.Dummy<Country>(), "test@sfwltd.co.uk", 0));
            A.CallTo(() => invoiceRun.IssuedDate)
                .Returns(new DateTime(2016, 02, 25));

            A.CallTo(() => dataAccess.FetchInvoiceRunAsync(A<Guid>._))
                .Returns(invoiceRun);

            var data = await handler.HandleAsync(A.Dummy<FetchInvoiceRunCsv>());

            Assert.Equal("invoicerun_NRW_25022016.csv", data.FileName);
        }
    }
}
