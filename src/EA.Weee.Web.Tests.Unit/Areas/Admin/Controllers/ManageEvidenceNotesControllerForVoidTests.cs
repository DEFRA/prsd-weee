namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using Core.Helpers;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Organisations;
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
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ManageEvidenceNotesControllerForVoidTests : ManageEvidenceNotesControllerTestsBase
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
        public void VoidEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidEvidenceNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void VoidTransferNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void VoidEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidEvidenceNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
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
        public void VoidEvidenceNoteGet_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidEvidenceNote", new[]
                {
                    typeof(Guid),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void VoidEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidEvidenceNote", new[]
                {
                    typeof(VoidEvidenceNoteViewModel),
                }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void VoidTransferNotePost_ShouldHaveHttpPostAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(VoidTransferNoteViewModel),
                }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void VoidEvidenceNotePost_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidEvidenceNote", new[]
                {
                    typeof(VoidEvidenceNoteViewModel),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void VoidTransferNotePost_ShouldHaveAuthorizeInternalClaimsAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("VoidTransferNote", new[]
                {
                    typeof(VoidTransferNoteViewModel),
                }).Should()
                .BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
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
        public async Task VoidEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            //act
            await ManageEvidenceController.VoidEvidenceNote(EvidenceNoteId);

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task VoidEvidenceNotePost_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task VoidTransferNotePost_BreadcrumbShouldBeSet()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();
            TransferEvidenceNoteData.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            vr.ViewName.Should().Be("VoidTransferNote");
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenInvalidModel_ViewShouldBeReturned()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(model);
            ViewResult vr = result as ViewResult;

            //assert
            vr.ViewName.Should().Be("VoidEvidenceNote");
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
        public async Task VoidEvidenceNoteGet_GivenEvidenceId_EvidenceNoteShouldBeRetrieved()
        {
            //act
            SetUpControllerContext(true);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            //act
            await ManageEvidenceController.VoidEvidenceNote(EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestDataIsNotTransferType_RedirectsToManageEvidenceNotes()
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
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }

        [Fact]
        public async Task VoidEvidenceNoteGet_GivenRequestDataIsNotEvidenceType_RedirectsToManageEvidenceNotes()
        {
            //arrange
            SetUpControllerContext(true);
            var evidenceNote = A.Fake<EvidenceNoteData>();
            A.CallTo(() => evidenceNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => evidenceNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(evidenceNote);

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(EvidenceNoteId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString());
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenRequestData_TransferEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            SetUpControllerContext(true);
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();
            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);

            //act
            await ManageEvidenceController.VoidTransferNote(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(transferNote) &&
                     v.DisplayNotification == null &&
                     v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidEvidenceNoteGet_GivenRequestData_EvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            SetUpControllerContext(true);
            var evidenceNoteData = A.Fake<EvidenceNoteData>();
            
            A.CallTo(() => evidenceNoteData.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => evidenceNoteData.Type).Returns(NoteType.Evidence);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(evidenceNoteData);

            //act
            await ManageEvidenceController.VoidEvidenceNote(EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(evidenceNoteData) &&
                     v.NoteStatus == null &&
                     v.User == null &&
                     v.PrintableVersion == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenInvalidViewModel_ModelShouldBeBuilt()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await ManageEvidenceController.VoidTransferNote(model);

            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                v => v.TransferEvidenceNoteData.Equals(TransferEvidenceNoteData) &&
                     v.DisplayNotification == null &&
                     v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenInvalidViewModel_ModelShouldBeBuilt()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await ManageEvidenceController.VoidEvidenceNote(model);

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(EvidenceNoteData) &&
                     v.NoteStatus == null &&
                     v.User == null &&
                     v.PrintableVersion == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenInvalidViewModel_ModelShouldBeReturned()
        {
            //act
            SetUpControllerContext(true);
            var viewTransferNoteModel = TestFixture.Create<ViewTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._))
                .Returns(viewTransferNoteModel);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await ManageEvidenceController.VoidTransferNote(TestFixture.Create<VoidTransferNoteViewModel>());
            ViewResult vr = result as ViewResult;

            //assert
            var convertedModel = (VoidTransferNoteViewModel)vr.Model;
            convertedModel.ViewTransferNoteViewModel.Should().Be(viewTransferNoteModel);
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenInvalidViewModel_ModelShouldBeReturned()
        {
            //act
            SetUpControllerContext(true);
            var viewEvidenceNoteViewModel = TestFixture.Create<ViewEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(EvidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._))
                .Returns(viewEvidenceNoteViewModel);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(TestFixture.Create<VoidEvidenceNoteViewModel>());
            ViewResult vr = result as ViewResult;

            //assert
            var convertedModel = (VoidEvidenceNoteViewModel)vr.Model;
            convertedModel.ViewEvidenceNoteViewModel.Should().Be(viewEvidenceNoteViewModel);
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenViewModel_VoidTransferNoteRequestShouldBeRun()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();

            //act
            await ManageEvidenceController.VoidTransferNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidNoteRequest>.That.Matches(v => v.NoteId == model.ViewTransferNoteViewModel.EvidenceNoteId && v.Reason.Equals(model.VoidedReason)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenInvalidViewModel_VoidTransferNoteRequestShouldNotBeRun()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(TransferEvidenceNoteData);

            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await ManageEvidenceController.VoidTransferNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidNoteRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenInvalidViewModel_VoidTransferNoteRequestShouldNotBeRun()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();
            ManageEvidenceController.ModelState.AddModelError("error", "error");

            //act
            await ManageEvidenceController.VoidEvidenceNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidNoteRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenViewModel_VoidNoteRequestShouldBeRun()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidNoteRequest>._)).Returns(model.ViewEvidenceNoteViewModel.Id);

            //act
            await ManageEvidenceController.VoidEvidenceNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<VoidNoteRequest>.That.Matches(v => v.NoteId == model.ViewEvidenceNoteViewModel.Id && v.Reason.Equals(model.VoidedReason)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task VoidTransferNotePost_GivenVoidedNote_VoidTransferNoteShouldRedirectToAction()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidTransferNoteViewModel>();

            //act
            var result = await ManageEvidenceController.VoidTransferNote(model) as RedirectToRouteResult;

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be(NoteUpdatedStatusEnum.Void);

            result.RouteValues["action"].Should().Be("ViewEvidenceNoteTransfer");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewTransferNoteViewModel.EvidenceNoteId);
        }

        [Fact]
        public async Task VoidEvidenceNotePost_GivenVoidedNote_VoidEvidenceNoteShouldRedirectToAction()
        {
            //act
            SetUpControllerContext(true);
            var model = TestFixture.Create<VoidEvidenceNoteViewModel>();

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(model) as RedirectToRouteResult;

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(NoteUpdatedStatusEnum.Void);

            result.RouteValues["action"].Should().Be("ViewEvidenceNote");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewEvidenceNoteViewModel.Id);
        }

        [Fact]
        public async Task VoidTransferNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewTransferNoteViewModel>();
            var transferNote = A.Fake<TransferEvidenceNoteData>();
            transferNote.TransferredOrganisationData = TestFixture.Create<OrganisationData>();

            A.CallTo(() => transferNote.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => transferNote.Type).Returns(NoteType.Transfer);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteTransfersForInternalUserRequest>._)).Returns(transferNote);
            A.CallTo(() => Mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.VoidTransferNote(EvidenceNoteId) as ViewResult;

            //asset
            var convertedModel = (VoidTransferNoteViewModel)result.Model;
            convertedModel.ViewTransferNoteViewModel.Should().Be(model);
        }

        [Fact]
        public async Task VoidEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            SetUpControllerContext(true);
            var model = TestFixture.Create<ViewEvidenceNoteViewModel>();
            var evidenceNoteData = A.Fake<EvidenceNoteData>();

            A.CallTo(() => evidenceNoteData.Status).Returns(NoteStatus.Approved);
            A.CallTo(() => evidenceNoteData.Type).Returns(NoteType.Evidence);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForInternalUserRequest>._)).Returns(evidenceNoteData);
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.VoidEvidenceNote(EvidenceNoteId) as ViewResult;

            //asset
            var convertedModel = (VoidEvidenceNoteViewModel)result.Model;
            convertedModel.ViewEvidenceNoteViewModel.Should().Be(model);
        }
    }
}
