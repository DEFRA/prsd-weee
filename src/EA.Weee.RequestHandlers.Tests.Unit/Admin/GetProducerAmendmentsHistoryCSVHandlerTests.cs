namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;

    public class GetProducerAmendmentsHistoryCSVHandlerTests
    {
        [Fact]
        public async Task GetProducerAmendmentsHistoryCSVHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var prn = "WEE/MM0001AA";

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetProducerAmendmentsHistoryCSVHandler_PRNIsNullOrEmpty_ThrowsArgumentException(string prn)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetProducerAmendmentsHistoryCSVHandler_ReturnsFileContent()
        {
            // Arrange
            var prn = "WEE/MM0001DD";

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }
    }
}
