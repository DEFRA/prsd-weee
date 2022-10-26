namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using AutoFixture;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Admin;
    using Web.Areas.Aatf.Controllers;
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence.Reports;
    using Xunit;

    public class ManageEvidenceNotesControllerDownloadEvidenceSummaryReportTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void DownloadEvidenceSummaryReport_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceSummaryReport", new[] {typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceSummaryReport_SummaryReportShouldBeRetrieved()
        {
            //arrange
            var aatfId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            //act
            await ManageEvidenceController.DownloadEvidenceSummaryReport(aatfId, complianceYear);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfSummaryReportRequest>.That.Matches(
                g => g.ComplianceYear == complianceYear && g.AatfId == aatfId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceSummaryReport_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            var aatfId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfSummaryReportRequest>._)).Returns(csvFile);

            //act
            var result = await ManageEvidenceController.DownloadEvidenceSummaryReport(aatfId, complianceYear) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
            result.FileDownloadName.Should().Be(csvFile.FileName);
        }
    }
}