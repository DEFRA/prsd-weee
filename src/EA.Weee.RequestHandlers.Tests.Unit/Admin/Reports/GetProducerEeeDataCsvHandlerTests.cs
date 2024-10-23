namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using Core.Admin;
    using Core.Shared;
    using EA.Prsd.Core;
    using EA.Weee.Core.Constants;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Reports.GetProducerEeeDataCsv;
    using RequestHandlers.Security;
    using Requests.Admin.Reports;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetProducerEeeDataCsvHandlerTests
    {
        [Fact]
        public async Task GetProducerEeeDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetProducerEeeDataCsvHandler handler = new GetProducerEeeDataCsvHandler(
                authorization,
                A.Dummy<IGetProducerEeeDataCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            GetProducerEeeDataCsv request = new GetProducerEeeDataCsv(complianceYear, null, ObligationType.B2B);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetProducerEeeDataCsvHandler_ComplianceYear_B2B_ReturnsFileContent()
        {
            // Arrange
            int complianceYear = 2016;

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerEeeDataCsvHandler handler = new GetProducerEeeDataCsvHandler(
                authorization,
                A.Dummy<IGetProducerEeeDataCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            GetProducerEeeDataCsv request = new GetProducerEeeDataCsv(complianceYear, null, ObligationType.B2B);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetProducerEeeDataCsvHandler_ComplianceYear_B2C_ReturnsFileContent()
        {
            // Arrange
            int complianceYear = 2016;

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            GetProducerEeeDataCsvHandler handler = new GetProducerEeeDataCsvHandler(
                authorization,
                A.Dummy<IGetProducerEeeDataCsvDataAccess>(),
                A.Dummy<CsvWriterFactory>());

            GetProducerEeeDataCsv request = new GetProducerEeeDataCsv(complianceYear, null, ObligationType.B2C);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task HandleAsync_WithDirectRegistrantId_GeneratesCorrectFileName()
        {
            // Arrange
            var time = new DateTime(2016, 1, 2, 11, 12, 30);
            SystemTime.Freeze(time);
            var request = new GetProducerEeeDataCsv(
                2016,
                DirectRegistrantFixedIdConstant.DirectRegistrantFixedId,
                ObligationType.B2B);

            var handler = new GetProducerEeeDataCsvHandler(
                AuthorizationBuilder.CreateUserWithAllRights(),
                A.Fake<IGetProducerEeeDataCsvDataAccess>(),
                A.Fake<CsvWriterFactory>());

            // Act
            var result = await handler.HandleAsync(request);
            
            // Assert
            result.FileName.Should().Be("2016_Direct registrant_B2B_producerEEE_02012016_1112.csv");

            SystemTime.Unfreeze();
        }
    }
}
