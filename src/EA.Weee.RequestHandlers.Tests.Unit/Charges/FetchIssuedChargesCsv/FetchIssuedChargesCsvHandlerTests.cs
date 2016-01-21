namespace EA.Weee.RequestHandlers.Tests.Unit.Charges.FetchIssuedChargesCsv
{
    using System.Security;
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
        }        
    }
}
