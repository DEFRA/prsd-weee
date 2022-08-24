namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using Core.Helpers;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Web.Areas.Admin.ViewModels.Shared;
    using Xunit;

    public class ManageEvidenceNotesControllerForViewEvidenceNoteTransferTests : ManageEvidenceNotesControllerTestsBase
    {
        private void SetUpControllerContext(bool hasInternalAdminUserClaims)
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            var principal = new ClaimsPrincipal(httpContextBase.User);
            var claimsIdentity = new ClaimsIdentity(httpContextBase.User.Identity);

            if (hasInternalAdminUserClaims)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.InternalAdmin));
            }
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => httpContextBase.User.Identity).Returns(claimsIdentity);

            var context = new ControllerContext(httpContextBase, new RouteData(), ManageEvidenceController);
            ManageEvidenceController.ControllerContext = context;
        }

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
        public void ViewEvidenceNoteTransferGet_ShouldHaveNoCacheFilterAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNoteTransfer", new[]
                {
                    typeof(Guid),
                    typeof(int)
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
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
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferPost_GivenInvalidModel_TransferEvidenceNoteShouldBeRetrieved()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();
            
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await ManageEvidenceController.VoidTransferNote(model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(model.ViewTransferNoteViewModel.EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenRequestData_TransferEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(TransferEvidenceNoteData) &&
                     v.DisplayNotification == null &&
                     v.User == ManageEvidenceController.HttpContext.User))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestDataIsNotApprovedStatus_RedirectsToManageEvidenceNotes()
        {
            //arrange
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Submitted);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ViewEvidenceNoteTransferGet_GivenNoteDataAndDisplayNotification_ModelMapperShouldBeCalled(bool displayNotification)
        {
            // arrange 
            SetUpControllerContext(true);
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
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }
    }
}
