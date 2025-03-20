namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchIssuedChargesCsv
{
    using System;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using FakeItEasy;
    using RequestHandlers.Charges.FetchIssuedChargesCsv;
    using RequestHandlers.Security;
    using Requests.Charges;
    using Weee.Tests.Core;
    using Xunit;

    public class FetchIssuedChargesCsvHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var dataAccess = A.Dummy<IFetchIssuedChargesCsvDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchIssuedChargesCsvHandler(authorization, dataAccess, csvWriterFactory);

            await Assert.ThrowsAsync<SecurityException>(() => handler.HandleAsync(A.Dummy<FetchIssuedChargesCsv>()));
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var dataAccess = A.Dummy<IFetchIssuedChargesCsvDataAccess>();
            var csvWriterFactory = A.Dummy<CsvWriterFactory>();

            var handler = new FetchIssuedChargesCsvHandler(authorization, dataAccess, csvWriterFactory);

            var data = await handler.HandleAsync(A.Dummy<FetchIssuedChargesCsv>());

            Assert.NotEmpty(data.Data);

            var result = Encoding.UTF8.GetString(data.Data);

            var lines = result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var expectedHeaders = string.Join(",", new[]
            {
            "Scheme name",
            "Compliance year",
            "Submission date and time (GMT)",
            "Producer name",
            "PRN",
            "Charge value (GBP)",
            "Charge band",
            "Selling technique",
            "Online market places charge value",
            "Issued date",
            "Reg. Off. or PPoB country",
            "Includes annual charge"
            });

            Assert.Equal(expectedHeaders, lines[0]);
        }
    }
}
