namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ManageEvidenceNotesControllerForViewEvidenceNoteTransferTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ViewEvidenceNoteTransferGet_ShouldHaveHttpGetAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNoteTransfer", new[]
                {
                    typeof(Guid),
                    typeof(int)
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_BreadcrumbShouldBeSet()
        {
            //act
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);
           
            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenDefaultPageSize_PageSizeShouldBeSetInViewBag()
        {
            //arrange
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //assert
            var page = (int)ManageEvidenceController.ViewBag.Page;
            page.Should().Be(1);
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenPageSize_PageSizeShouldBeSetInViewBag()
        {
            //arrange
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            const int pageSize = 10;

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId, pageSize);

            //assert
            var page = (int)ManageEvidenceController.ViewBag.Page;
            page.Should().Be(pageSize);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenDefaultPageSize_PageSizeShouldBeSetInViewBag()
        {
            //arrange

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId);

            //assert
            var page = (int)ManageEvidenceController.ViewBag.Page;
            page.Should().Be(1);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenPageSize_PageSizeShouldBeSetInViewBag()
        {
            //arrange
            const int pageSize = 10;

            //act
            await ManageEvidenceController.ViewEvidenceNote(EvidenceNoteId, pageSize);

            //assert
            var page = (int)ManageEvidenceController.ViewBag.Page;
            page.Should().Be(pageSize);
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenEvidenceId_TransferEvidenceNoteShouldBeRetrieved()
        {
            //act
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenRequestDat_TransferEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(TransferEvidenceNoteData) &&
                     v.DisplayNotification == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ViewEvidenceNoteTransferGet_GivenNoteDataAndDisplayNotification_ModelMapperShouldBeCalled(bool displayNotification)
        {
            // arrange 
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = displayNotification;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            // act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.TransferEvidenceNoteData.Equals(TransferEvidenceNoteData) &&
                         t.DisplayNotification.Equals(displayNotification))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_WhenNoteTypeIsTransfer_ModelWithRedirectTabIsCreated()
        {
            // act
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            var result = await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId) as ViewResult;

            // act
            var model = result.Model as ViewTransferNoteViewModel;
            model.Type = NoteType.Transfer;

            //assert
            model.InternalUserRedirectTab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }
    }
}
