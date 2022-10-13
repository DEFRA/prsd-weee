namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Web.Infrastructure;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Controllers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests : ManageEvidenceNotesControllerTestsBase
    {
        public ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests()
        {
            var model = Fixture.Build<ViewEvidenceNoteViewModel>()
                .With(e => e.Reference, 1)
                .With(e => e.Type, NoteType.Evidence)
                .Create();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);
        }

        [Fact]
        public void DownloadEvidenceNotesReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNotesReport", new[] {typeof(Guid), typeof(int), typeof(TonnageToDisplayReportEnum) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            Guid? aatfId = Fixture.Create<Guid?>();
            var complianceYear = Fixture.Create<int>();
            var tonnageToDisplay = Fixture.Create<TonnageToDisplayReportEnum>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(csvFile);

            //act
            await ManageEvidenceController.DownloadEvidenceNotesReport(aatfId, complianceYear, tonnageToDisplay);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYear &&
                g.OriginatorOrganisationId == null &&
                g.RecipientOrganisationId == null &&
                g.AatfId == aatfId &&
                g.TonnageToDisplay == tonnageToDisplay &&
                g.InternalRequest == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            Guid? aatfId = Fixture.Create<Guid?>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            var tonnageDisplay = Fixture.Create<TonnageToDisplayReportEnum>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(csvFile);

            //act
            var result = await ManageEvidenceController.DownloadEvidenceNotesReport(aatfId, complianceYear, tonnageDisplay) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
            result.FileDownloadName.Should().Be(csvFile.FileName);
        }
    }
}