namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using AutoFixture;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Requests.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUkNonObligatedWeeeReceivedDataCsvHandlerTests
    {
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IStoredProcedures storedProcedures;
        private readonly GetUkNonObligatedWeeeReceivedDataCsvHandler handler;
        private readonly Fixture fixture;

        public GetUkNonObligatedWeeeReceivedDataCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            storedProcedures = A.Fake<IStoredProcedures>();
            fixture = new Fixture();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(
                new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                context,
                csvWriterFactory);
        }

        [Fact]
        public async Task HandleAsync_NotInternalUser_ThrowsSecurityException()
        {
            const int complianceYear = 2016;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_ComplianceYear_ReturnsFileContent()
        {
            const int complianceYear = 2016;

            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            var data = await handler.HandleAsync(request);

            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task HandleAsync_Returns_MatchingFileContent()
        {
            const int complianceYear = 2019;
            const string quarter = "Q1";

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
            .GetUkNonObligatedWeeeReceivedByComplianceYear(A<int>._))
            .Returns(new List<UkNonObligatedWeeeReceivedData> { csvData1, csvData2, csvData3 });

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
        public async Task HandleAsync_PassVariousComplianceYears_ReturnsFileContent(int complianceYear)
        {
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            // Act
            var data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndLocalArea_FileNameShouldBeCorrect()
        {
            var request = new GetUkNonObligatedWeeeReceivedDataCsv(fixture.Create<int>());

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{ request.ComplianceYear}_UK non-obligated WEEE received at AATFs_{ date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }
    }
}
