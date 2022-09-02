namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Areas.Aatf.Controllers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class ManageEvidenceNotesControllerDownloadEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        public ManageEvidenceNotesControllerDownloadEvidenceNoteTests()
        {
            var model = Fixture.Build<ViewEvidenceNoteViewModel>()
                .With(e => e.Reference, 1)
                .With(e => e.Type, NoteType.Evidence)
                .Create();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] {typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_GivenEvidenceId_EvidenceNoteShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.DownloadEvidenceNote(EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_GivenRequestData_EvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            var data = Fixture.Create<EvidenceNoteData>();
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = null;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(data);

            //act
            await ManageEvidenceController.DownloadEvidenceNote(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(data) &&
                     v.NoteStatus == null &&
                     v.PrintableVersion == true &&
                     v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_GivenEvidenceNoteViewModel_ContentShouldBeRenderedFromView()
        {
            //arrange
            var model = Fixture.Create<ViewEvidenceNoteViewModel>();
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            //act
            await ManageEvidenceController.DownloadEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => TemplateExecutor.RenderRazorView(ManageEvidenceController.ControllerContext,
                "DownloadEvidenceNote",
                model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_GivenPdfContent_PdfShouldBeCreated()
        {
            //arrange
            var content = Fixture.Create<string>();
            A.CallTo(() => TemplateExecutor.RenderRazorView(A<ControllerContext>._, A<string>._, A<object>._)).Returns(content);

            //act
            await ManageEvidenceController.DownloadEvidenceNote(EvidenceNoteId);

            //assert
            A.CallTo(() => PdfDocumentProvider.GeneratePdfFromHtml(content, null)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_GivenPdf_FileShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 09, 2, 13, 22, 0);
            SystemTime.Freeze(date);
            var pdf = Fixture.Create<byte[]>();

            A.CallTo(() => PdfDocumentProvider.GeneratePdfFromHtml(A<string>._, null)).Returns(pdf);

            //act
            var result = await ManageEvidenceController.DownloadEvidenceNote(EvidenceNoteId) as FileContentResult;

            //assert
            result.FileContents.Should().BeSameAs(pdf);
            result.FileDownloadName.Should().Be("E1_2022_02/09/2022_1422.pdf");
            result.ContentType.Should().Be("application/pdf");
            SystemTime.Unfreeze();
        }
    }
}