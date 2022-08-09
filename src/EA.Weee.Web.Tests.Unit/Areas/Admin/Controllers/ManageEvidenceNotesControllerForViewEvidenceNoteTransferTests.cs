namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using EA.Weee.Api.Client.Actions;
    using EA.Weee.Api.Client.Entities;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
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
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

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
        public async Task ViewEvidenceNoteTransferGet_GivenRequestDat_TransferEvidenceNoteModelShouldBeBuilt()
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
    }
}
