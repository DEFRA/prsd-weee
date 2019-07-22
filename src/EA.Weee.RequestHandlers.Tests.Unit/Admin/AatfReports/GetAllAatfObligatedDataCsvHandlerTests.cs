namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Shared;
    using FakeItEasy;
    using RequestHandlers.Admin.Reports;
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

        [Fact]
        public async Task GetAllAatfObligatedDataCsvHandler_NoColumnName_ReturnsFileContent()
        {
            // Arrange
            var complianceYear = 2019;

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
    }
}
