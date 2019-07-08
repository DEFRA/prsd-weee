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
        public async Task GetAatfAeReturnDataCsvHandler_NoQuarter_ReturnsFileContent()
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

        [Fact]
        public async Task GetAatfAeReturnDataCSVHandler_Returns_MatchingFileContent()
        {
            var complianceYear = 2019;
            int quarter = 1;
            FacilityType facilityType = FacilityType.Aatf;
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var storedProcedures = A.Fake<IStoredProcedures>();
           
            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            var csvData1 = new AatfAeReturnData
            {
                Name = "aatf1", ApprovalNumber = "WEE/EE1234RR/ATF", OrganisationName = "Test Org"
            };

            var csvData2 = new AatfAeReturnData
            {
                Name = "aatf12",
                ApprovalNumber = "WEE/EE1234RR/ATF",
                OrganisationName = "Test Org"
            };

            var csvData3 = new AatfAeReturnData
            {
                Name = "aatf3",
                ApprovalNumber = "WEE/EE1234RR/ATF",
                OrganisationName = "Test Org"
            };

            A.CallTo(() => storedProcedures
            .GetAatfAeReturnDataCsvData(A<int>._, A<int>._, A<int>._, null, null, null, null))
            .Returns(new List<AatfAeReturnData> { csvData1, csvData2, csvData3 });

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, A.Dummy<CsvWriterFactory>());
            var request = new GetAatfAeReturnDataCsv(complianceYear, quarter, facilityType, null, null, null, null);

            // Act
            var data = await handler.HandleAsync(request);
            data.FileContent.Contains("aatf1,WEE/EE1234RR/ATF,Test Org,,,,,");
            data.FileContent.Contains("aatf12,WEE/EE1234RR/ATF,Test Org,,,,,");
            data.FileContent.Contains("aatf3,WEE/EE1234RR/ATF,Test Org,,,,,");
        }
    }
}
