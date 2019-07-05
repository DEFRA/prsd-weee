namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfAeReturnDataCsvHandlerTests
    {
        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetAatfAeReturnDataCsv(complianceYear, 1, FacilityType.Aatf, null, null, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetAatfAeReturnDataCsv(complianceYear, 1, FacilityType.Aatf, null, null, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_NoQuater_ReturnsFileContent()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetAatfAeReturnDataCsv(complianceYear, 0, FacilityType.Aatf, null, null, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_ComplianceYear_ReturnsFileContent()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetAatfAeReturnDataCsv(complianceYear, 1, FacilityType.Aatf, null, null, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Theory]
        [InlineData(2019, 1, FacilityType.Aatf, null, null, null, null)]
        [InlineData(2019, 2, FacilityType.Aatf, null, null, null, null)]
        [InlineData(2019, 3, FacilityType.Aatf, null, null, null, null)]
        [InlineData(2019, 4, FacilityType.Aatf, null, null, null, null)]
        [InlineData(2019, 1, FacilityType.Ae, null, null, null, null)]
        [InlineData(2019, 2, FacilityType.Ae, null, null, null, null)]
        [InlineData(2019, 3, FacilityType.Ae, null, null, null, null)]
        [InlineData(2019, 4, FacilityType.Ae, null, null, null, null)]
        [InlineData(2019, 1, FacilityType.Aatf, 2, null, null, null)]
        [InlineData(2019, 1, FacilityType.Aatf, 1, null, null, null)]
        [InlineData(2019, 1, FacilityType.Aatf, 0, null, null, null)]
        [InlineData(2019, 1, FacilityType.Ae, 2, null, null, null)]
        [InlineData(2019, 1, FacilityType.Ae, 1, null, null, null)]
        [InlineData(2019, 1, FacilityType.Ae, 0, null, null, null)]
        public async Task GetAatfAeReturnDataCsvHandler_VariousParameters_ReturnsFileContent(int complianceYear, int quarter,
            FacilityType facilityType, int? returnStatus, Guid? authority, Guid? area, Guid? panArea)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetAatfAeReturnDataCsv(complianceYear, quarter, facilityType, returnStatus, null, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }
    }
}
