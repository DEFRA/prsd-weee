namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.Admin.AatfReports;
    using RequestHandlers.Shared;
    using Requests.Admin.AatfReports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetPcsAatfComparisonDataHandlerTests
    {
        private readonly GetPcsAatfComparisonDataHandler handler;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly Fixture fixture;
        private readonly IStoredProcedures storedProcedures;
        private readonly ICommonDataAccess commonDataAccess;

        public GetPcsAatfComparisonDataHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            fixture = new Fixture();
            storedProcedures = A.Fake<IStoredProcedures>();
            commonDataAccess = A.Fake<ICommonDataAccess>();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            handler = new GetPcsAatfComparisonDataHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                context,
                csvWriterFactory,
                commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NotInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetPcsAatfComparisonDataHandler(authorization, context, csvWriterFactory, commonDataAccess);
            var request = new GetPcsAatfComparisonData(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;
            var request = new GetPcsAatfComparisonData(complianceYear, fixture.Create<int>(), fixture.Create<string>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_VariousParameters_ReturnsFileContent()
        {
            var request = new GetPcsAatfComparisonData(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>());

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task HandleAsync_GivenStoredProcedureReturnItems_MatchingFileContent()
        {
            var complianceYear = fixture.Create<int>();
            var quarter = fixture.Create<int>();
            var obligationType = fixture.Create<string>();

            var csvData1 = new PcsAatfComparisonDataCsvData
            {
                ComplianceYear = fixture.Create<int>(),
                QuarterValue = fixture.Create<string>(),
                ObligationType = fixture.Create<string>(),
                Category = fixture.Create<string>(),
                SchemeName = fixture.Create<string>(),
                PcsApprovalNumber = fixture.Create<string>(),
                PcsAbbreviation = fixture.Create<string>(),
                AatfName = fixture.Create<string>(),
                AatfApprovalNumber = fixture.Create<string>(),
                AatfAbbreviation = fixture.Create<string>(),
                PcsTonnage = fixture.Create<decimal>(),
                AatfTonnage = fixture.Create<decimal>(),
                DifferenceTonnage = fixture.Create<decimal>()
            };

            var csvData2 = new PcsAatfComparisonDataCsvData
            {
                ComplianceYear = fixture.Create<int>(),
                QuarterValue = fixture.Create<string>(),
                ObligationType = fixture.Create<string>(),
                Category = fixture.Create<string>(),
                SchemeName = fixture.Create<string>(),
                PcsApprovalNumber = fixture.Create<string>(),
                PcsAbbreviation = fixture.Create<string>(),
                AatfName = fixture.Create<string>(),
                AatfApprovalNumber = fixture.Create<string>(),
                AatfAbbreviation = fixture.Create<string>(),
                PcsTonnage = fixture.Create<decimal>(),
                AatfTonnage = fixture.Create<decimal>(),
                DifferenceTonnage = fixture.Create<decimal>()
            };

            var csvData3 = new PcsAatfComparisonDataCsvData
            {
                ComplianceYear = fixture.Create<int>(),
                QuarterValue = fixture.Create<string>(),
                ObligationType = fixture.Create<string>(),
                Category = fixture.Create<string>(),
                SchemeName = fixture.Create<string>(),
                PcsApprovalNumber = fixture.Create<string>(),
                PcsAbbreviation = fixture.Create<string>(),
                AatfName = fixture.Create<string>(),
                AatfApprovalNumber = fixture.Create<string>(),
                AatfAbbreviation = fixture.Create<string>(),
                PcsTonnage = fixture.Create<decimal>(),
                AatfTonnage = fixture.Create<decimal>(),
                DifferenceTonnage = fixture.Create<decimal>()
            };

            A.CallTo(() => storedProcedures.GetPcsAatfComparisonDataCsvData(complianceYear, quarter, obligationType))
            .Returns(new List<PcsAatfComparisonDataCsvData> { csvData1, csvData2, csvData3 });

            var request = new GetPcsAatfComparisonData(complianceYear, quarter, obligationType);

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().Contain("Compliance year,Quarter,Obligation type,Category,PCS name,PCS approval number,PCS appropriate authority,AATF name,AATF approval number,AATF appropriate authority,PCS report (t),AATF report(t),Discrepancy between PCS and AATF reports (t)");
            data.FileContent.Should().Contain($"{csvData1.ComplianceYear},{csvData1.QuarterValue},{csvData1.ObligationType},{csvData1.Category},{csvData1.SchemeName},{csvData1.PcsApprovalNumber},{csvData1.PcsAbbreviation},{csvData1.AatfName},{csvData1.AatfApprovalNumber},{csvData1.AatfAbbreviation},{csvData1.PcsTonnage},{csvData1.AatfTonnage},{csvData1.DifferenceTonnage}");
            data.FileContent.Should().Contain($"{csvData2.ComplianceYear},{csvData2.QuarterValue},{csvData2.ObligationType},{csvData2.Category},{csvData2.SchemeName},{csvData2.PcsApprovalNumber},{csvData2.PcsAbbreviation},{csvData2.AatfName},{csvData2.AatfApprovalNumber},{csvData2.AatfAbbreviation},{csvData2.PcsTonnage},{csvData2.AatfTonnage},{csvData2.DifferenceTonnage}");
            data.FileContent.Should().Contain($"{csvData3.ComplianceYear},{csvData3.QuarterValue},{csvData3.ObligationType},{csvData3.Category},{csvData3.SchemeName},{csvData3.PcsApprovalNumber},{csvData3.PcsAbbreviation},{csvData3.AatfName},{csvData3.AatfApprovalNumber},{csvData3.AatfAbbreviation},{csvData3.PcsTonnage},{csvData3.AatfTonnage},{csvData3.DifferenceTonnage}");
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParameters_FileNameShouldBeCorrect()
        {
            var request = new GetPcsAatfComparisonData(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<string>());

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{request.ObligationType}_PCS v AATF WEEE data comparison_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }
    }
}
