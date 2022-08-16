namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
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
        public void VoidTransferNoteGet_ShouldHaveHttpGetAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void VoidTransferNoteGet_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void VoidTransferNotePost_ShouldHaveHttpPostAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(ViewTransferNoteViewModel),
                }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void VoidTransferNotePost_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(ViewTransferNoteViewModel),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
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
        public async Task VoidTransferNoteGet_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task VoidTransferNotePost_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
            vr.ViewName.Should().Be("VoidTransferNote");
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
        public async Task ViewEvidenceNoteTransferPost_GivenEvidenceId_TransferEvidenceNoteShouldBeRetrieved()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = NoteUpdatedStatusEnum.Void;
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.VoidTransferNote(model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(model.EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenEvidenceId_TransferEvidenceNoteShouldBeRetrieved()
        {
            //act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_GivenRequestData_TransferEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            SetUpControllerContext(true);
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(TransferEvidenceNoteData) &&
                     v.DisplayNotification == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestDataIsNotApprovedStatus_RaisesHttpForbiddenError()
        {
            //arrange
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Submitted);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //asset
            result.Should().BeOfType<HttpStatusCodeResult>();
            ((HttpStatusCodeResult)result).StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((HttpStatusCodeResult)result).StatusDescription.Should().Be("This note is not a transfer note or has not been approved.");
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestDataIsNotTransferType_RaisesHttpForbiddenError()
        {
            //arrange
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Evidence);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //asset
            result.Should().BeOfType<HttpStatusCodeResult>();
            ((HttpStatusCodeResult)result).StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            ((HttpStatusCodeResult)result).StatusDescription.Should().Be("This note is not a transfer note or has not been approved.");
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestData_TransferEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(transferNote)))).MustHaveHappenedOnceExactly();
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ViewEvidenceNoteTransferGet_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            // arrange 
            SetUpControllerContext(userHasInternalAdminClaims);
            var model = TestFixture.Build<ViewTransferNoteViewModel>().With(c => c.CanVoid, userHasInternalAdminClaims).Create();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId) as ViewResult;
            var resultViewModel = result.Model as ViewTransferNoteViewModel;

            // assert
            Assert.Equal(userHasInternalAdminClaims, resultViewModel.CanVoid);
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenNoteDataAndDisplayNotification_ModelMapperShouldBeCalled()
        {
            // arrange 
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            // act
            await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.TransferEvidenceNoteData.Equals(transferNote))))
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

        [Fact]
        public async Task VoidTransferNotePost_GivenViewModel_ModelShouldBeReturned()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            vr.Model.Should().BeOfType<ViewTransferNoteViewModel>();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenViewModel_VoidTransferNoteRequestShouldBeRun()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidTransferNoteRequest>._)).Returns(model.EvidenceNoteId);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidTransferNoteRequest>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenViewModel_VoidTransferNoteShouldRedirectToAction()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Id).Returns(model.EvidenceNoteId);
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Void);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidTransferNoteRequest>._)).Returns(model.EvidenceNoteId);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model) as RedirectToRouteResult;
        
            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidTransferNoteRequest>._)).MustHaveHappenedOnceExactly();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Equals(NoteUpdatedStatusEnum.Void);

            result.RouteValues["action"].Should().Be("ViewEvidenceNoteTransfer");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["evidenceNoteId"].Should().Be(model.EvidenceNoteId);
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task VoidTransferNoteGet_CheckUserCanVoid_ViewModelShouldBeSetUpCorrectly(bool userHasInternalAdminClaims)
        {
            //arrange
            SetUpControllerContext(userHasInternalAdminClaims);
            var model = TestFixture.Build<ViewTransferNoteViewModel>().With(x => x.CanVoid, userHasInternalAdminClaims).Create();
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as ViewResult;
            var resultViewModel = result.Model as ViewTransferNoteViewModel;

            //asset
            Assert.Equal(userHasInternalAdminClaims, resultViewModel.CanVoid);
        }

        [Fact]
        public async Task ViewEvidenceNoteTransferGet_WhenNoteTypeIsTransfer_ModelWithRedirectTabIsCreated()
        {
            // act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            var result = await ManageEvidenceController.ViewEvidenceNoteTransfer(EvidenceNoteId) as ViewResult;
            var model = result.Model as ViewTransferNoteViewModel;
            model.Type = NoteType.Transfer;

            //assert
            model.InternalUserRedirectTab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }

        [Fact]
        public async Task VoidTransferNoteGet_WhenNoteTypeIsTransfer_ModelWithRedirectTabIsCreated()
        {
            // act
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = null;
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as ViewResult;
            var model = result.Model as ViewTransferNoteViewModel;
            model.Type = NoteType.Transfer;

            //assert
            model.InternalUserRedirectTab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }
    }
}
