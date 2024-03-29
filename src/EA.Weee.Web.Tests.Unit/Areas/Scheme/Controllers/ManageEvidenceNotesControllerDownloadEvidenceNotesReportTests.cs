﻿namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controller
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using EA.Prsd.Email;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Controllers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests : ManageEvidenceNotesControllerTestsBase
    {
        protected readonly ManageEvidenceNotesController Controller;

        public ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests()
        {
            A.CallTo(() => ConfigurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            var content = Fixture.Create<string>();
            A.CallTo(() => TemplateExecutor.RenderRazorView(A<ControllerContext>._, A<string>._, A<object>._)).Returns(content);

            var model = Fixture.Build<ViewEvidenceNoteViewModel>()
                .With(e => e.Reference, 1)
                .With(e => e.Type, NoteType.Evidence)
                .Create();
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            Controller = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, SessionService, TemplateExecutor, PdfDocumentProvider, ConfigurationService);
        }

        [Fact]
        public void DownloadEvidenceNotesReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNotesReport", new[] { typeof(Guid), typeof(int), typeof(TonnageToDisplayReportEnum) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var tonnageToDisplay = Fixture.Create<TonnageToDisplayReportEnum>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(csvFile);

            //act
            await Controller.DownloadEvidenceNotesReport(pcsId, complianceYear, tonnageToDisplay);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYear &&
                g.OriginatorOrganisationId == null &&
                g.RecipientOrganisationId == pcsId &&
                g.AatfId == null &&
                g.TonnageToDisplay == tonnageToDisplay &&
                g.InternalRequest == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            var tonnageDisplay = Fixture.Create<TonnageToDisplayReportEnum>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteReportRequest>._)).Returns(csvFile);

            //act
            var result = await Controller.DownloadEvidenceNotesReport(pcsId, complianceYear, tonnageDisplay) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
            result.FileDownloadName.Should().Be(csvFile.FileName);
        }

        [Fact]
        public void DownloadEvidenceNotePdfGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNotePdf", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceNotePdfGet_GivenPdf_FileShouldBeReturned()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2022, 10, 5, 13, 22, 10));
            var pdf = Fixture.Create<byte[]>();

            var data = Fixture.Build<EvidenceNoteData>()
                .With(e => e.ComplianceYear, 2022)
                .Create();

            var model = Fixture.Build<ViewEvidenceNoteViewModel>()
                .With(v => v.Reference, 1200)
                .With(v => v.Type, NoteType.Evidence).Create();

            var content = Fixture.Create<string>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId == EvidenceNoteId))).Returns(data);
            A.CallTo(() => TemplateExecutor.RenderRazorView(A<ControllerContext>._, "DownloadEvidenceNotePdf", model)).Returns(content);
            A.CallTo(() => PdfDocumentProvider.GeneratePdfFromHtml(content, null)).Returns(pdf);
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(h => h.EvidenceNoteData == data))).Returns(model);

            //act
            var result = await Controller.DownloadEvidenceNotePdf(EvidenceNoteId) as FileContentResult;

            //assert
            result.FileContents.Should().BeSameAs(pdf);
            result.FileDownloadName.Should().Be("2022_E1200_051022_1422.pdf");
            result.ContentType.Should().Be("application/pdf");
            SystemTime.Unfreeze();
        }

        [Fact]
        public void DownloadEvidenceTransfersReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceTransfersReport", new[] { typeof(Guid), typeof(int) })
                .Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceTransfersReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);

            //act
            await Controller.DownloadEvidenceTransfersReport(pcsId, complianceYear);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYear &&
                g.OrganisationId == pcsId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceTransfersReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetTransferNoteReportRequest>._)).Returns(csvFile);

            //act
            var result = await Controller.DownloadEvidenceTransfersReport(pcsId, complianceYear) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
            result.FileDownloadName.Should().Be(csvFile.FileName);
        }

        [Fact]
        public void DownloadEvidenceSummaryReportGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceSummaryReport", new[] { typeof(Guid), typeof(int) })
                .Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceSummaryReportGet_GivenRouteValues_ReportRequestShouldBeCalled()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeObligationAndEvidenceTotalsReportRequest>._)).Returns(csvFile);

            //act
            await Controller.DownloadEvidenceSummaryReport(pcsId, complianceYear);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeObligationAndEvidenceTotalsReportRequest>.That.Matches(g =>
                                                g.SchemeId == null && 
                                                g.AppropriateAuthorityId == null && 
                                                g.OrganisationId == pcsId &&
                                                g.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceSummaryReportGet_GivenCsvData_FileContentResultShouldBeReturned()
        {
            //arrange
            Guid pcsId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var csvFile = Fixture.Create<CSVFileData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeObligationAndEvidenceTotalsReportRequest>._)).Returns(csvFile);

            //act
            var result = await Controller.DownloadEvidenceSummaryReport(pcsId, complianceYear) as FileContentResult;

            //assert
            result.FileContents.Should().BeEquivalentTo(new UTF8Encoding().GetBytes(csvFile.FileContent));
            result.FileDownloadName.Should().Be(CsvFilenameFormat.FormatFileName(csvFile.FileName));
            result.FileDownloadName.Should().Be(csvFile.FileName);
        }
    }
}
