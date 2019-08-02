namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Weee.RequestHandlers.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin.AatfReports;
    using Requests.Admin.Reports;
    using Weee.Tests.Core;
    using Xunit;
    public class GetAllAatfObligatedDataCsvHandlerTests
    {      
        [Fact]
        public async Task GetAllAatfObligatedDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, 1, string.Empty, string.Empty, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAllAatfObligatedDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, 1, string.Empty, string.Empty, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Theory]
        [InlineData(2019)]
        [InlineData(2020)]
        [InlineData(2021)]
        public async Task GetAllAatfObligatedDataCsvHandler_NoColumnName_ReturnsFileContent(int complianceYear)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, 0, string.Empty, string.Empty, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Theory]
        [InlineData(2019, 1, "", "", null, null)]
        [InlineData(2019, 2, "", "", null, null)]
        [InlineData(2019, 1, "B2B", "", null, null)]
        [InlineData(2019, 2, "B2B", "", null, null)]
        [InlineData(2019, 1, "B2C", "", null, null)]
        [InlineData(2019, 2, "B2C", "", null, null)]
        [InlineData(2019, 1, "", "A", null, null)]
        [InlineData(2019, 2, "", "A", null, null)]
        public async Task GetAllAatfObligatedDataCsvHandler_VariousParameters_ReturnsFileContent(int complianceYear, int columnType,
           string obligationType, string aatfName, Guid? authority, Guid? panArea)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, columnType, obligationType, aatfName, authority, panArea);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAllAatfObligatedDataCsvHandler_MandatoryParameters_ReturnsFileName()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            int complianceYear = 2019;
                
            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, 1, string.Empty, string.Empty, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.Contains("2019", data.FileName);
        }

        [Fact]
        public async Task GetAllAatfObligatedDataCsvHandler_StringParameters_ReturnsFileName()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            int complianceYear = 2019;
            string obligationType = "B2C";
            string aatfName = "A1";
            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var handler = new GetAllAatfObligatedDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfObligatedDataCsv(complianceYear, 1, obligationType, aatfName, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            data.FileName.Should().Be("2019_B2C_01022019_1101.csv");
        }
    }
}
