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
    using FluentAssertions;
    using Requests.Admin.AatfReports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUkNonObligatedWeeeReceivedDataCsvHandlerTests
    {
        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            const int complianceYear = 2016;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_ComplianceYear_ReturnsFileContent()
        {
            const int complianceYear = 2016;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            var data = await handler.HandleAsync(request);

            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_Returns_MatchingFileContent()
        {
            const int complianceYear = 2019;
            const string quarter = "Q1";
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            var csvData1 = new UkNonObligatedWeeeReceivedData
            {
                Category = "1. Large Household Appliances",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 99,
                TotalNonObligatedWeeeReceivedFromDcf = 98
            };

            var csvData2 = new UkNonObligatedWeeeReceivedData
            {
                Category = "2. Small Household Appliances",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 97,
                TotalNonObligatedWeeeReceivedFromDcf = 96
            };

            var csvData3 = new UkNonObligatedWeeeReceivedData
            {
                Category = "3. IT and Telecomms Equipment",
                Quarter = quarter,
                TotalNonObligatedWeeeReceived = 95,
                TotalNonObligatedWeeeReceivedFromDcf = 94
            };

            A.CallTo(() => storedProcedures
            .UkNonObligatedWeeeReceivedByComplianceYear(A<int>._))
            .Returns(new List<UkNonObligatedWeeeReceivedData> { csvData1, csvData2, csvData3 });

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, A.Dummy<CsvWriterFactory>());
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            var data = await handler.HandleAsync(request);
            data.FileContent.Should().Contain("Q1,1. Large Household Appliances,99,98");
            data.FileContent.Should().Contain("Q1,2. Small Household Appliances,97,96");
            data.FileContent.Should().Contain("Q1,3. IT and Telecomms Equipment,95,94");
        }

        [Theory]
        [InlineData(2019)]
        [InlineData(2020)]
        [InlineData(2021)]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandler_PassVariousComplianceYears_ReturnsFileContent(int complianceYear)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            var data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            const int complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
