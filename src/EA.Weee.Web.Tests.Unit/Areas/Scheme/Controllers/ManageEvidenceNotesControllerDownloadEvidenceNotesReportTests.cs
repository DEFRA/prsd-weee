namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controller
{
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.AatfEvidence.Reports;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Controllers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests //: ManageEvidenceNotesControllerTestsBase
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly Fixture Fixture;
        protected readonly IMapper Mapper;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly ISessionService SessionService;
        protected readonly ConfigurationService ConfigurationService;
        protected readonly ManageEvidenceNotesController Controller;

        public ManageEvidenceNotesControllerDownloadEvidenceNotesReportTests()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Fixture = new Fixture();
            Mapper = A.Fake<IMapper>();
            SessionService = A.Fake<ISessionService>();
            ConfigurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => ConfigurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);

            var model = Fixture.Build<ViewEvidenceNoteViewModel>()
                .With(e => e.Reference, 1)
                .With(e => e.Type, NoteType.Evidence)
                .Create();
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            Controller = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, SessionService, ConfigurationService);
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
    }
}