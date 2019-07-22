namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Aatf;
    using FakeItEasy;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUkNonObligatedWeeeReceivedDataCsvHandlerTests
    {
        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            int complianceYear = 0;

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_ComplianceYear_ReturnsFileContent()
        {
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            CSVFileData data = await handler.HandleAsync(request);

            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_Returns_MatchingFileContent()
        {
            int complianceYear = 2019;
            int quarter = 1;
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            IStoredProcedures storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            UkNonObligatedWeeeReceivedData csvData1 = new UkNonObligatedWeeeReceivedData
            {
                Category = "1. Large Household Appliances",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 99,
                TotalNonObligatedWeeeReceivedFromDcf = 98
            };

            UkNonObligatedWeeeReceivedData csvData2 = new UkNonObligatedWeeeReceivedData
            {
                Category = "2. Small Household Appliances",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 97,
                TotalNonObligatedWeeeReceivedFromDcf = 96
            };

            UkNonObligatedWeeeReceivedData csvData3 = new UkNonObligatedWeeeReceivedData
            {
                Category = "3. IT and Telecomms Equipment",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 95,
                TotalNonObligatedWeeeReceivedFromDcf = 94
            };

            A.CallTo(() => storedProcedures
            .UKUkNonObligatedWeeeReceivedByComplianceYear(A<int>._))
            .Returns(new List<UkNonObligatedWeeeReceivedData> { csvData1, csvData2, csvData3 });

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, A.Dummy<CsvWriterFactory>());
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            CSVFileData data = await handler.HandleAsync(request);
            data.FileContent.Contains("1,1. Large Household Appliances,99,98");
            data.FileContent.Contains("1,2. Small Household Appliances,97,96");
            data.FileContent.Contains("1,3. IT and Telecomms Equipment,95,84");
        }

        [Theory]
        [InlineData(2019)]
        [InlineData(2020)]
        [InlineData(2021)]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_PassVariousComplianceYears_ReturnsFileContent(int complianceYear)
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
