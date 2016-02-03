namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using Requests.Admin;
    using Xunit;

    public class GetProducerEeeDataHistoryCsvHandlerTests
    {
        /// <summary>
        /// This test ensures that the handler throws a security exception if used by
        /// a user without access to the internal area.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HandleAsync_WithNonInternalUser_ThrowsSecurityException()
        {
            // Arrange            
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv(A.Dummy<string>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNoRegistrationNumber_ThrowsArgumentException()
        {
            // Arrange            
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv(string.Empty);

            // Act
            Func<Task<CSVFileData>> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFileContent()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            GetProducerEeeDataHistoryCsvHandler handler = new GetProducerEeeDataHistoryCsvHandler(
                authorization,
                context,
                csvWriterFactory);

            GetProducerEeeDataHistoryCsv request = new GetProducerEeeDataHistoryCsv("PRN");
            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }
    }
}
