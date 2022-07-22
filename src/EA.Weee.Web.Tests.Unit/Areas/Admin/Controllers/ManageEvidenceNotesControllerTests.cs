namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ManageEvidenceNotesControllerTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ManageEvidenceNotesControllerInheritsAdminBreadcrumbBaseController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(AdminBreadcrumbBaseController));
        }

        [Fact]
        public void IndexGet_ShouldHaveHttpGetAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
                {
                    typeof(string),
                    typeof(ManageEvidenceNoteViewModel)
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        [InlineData("view-all-evidence-transfers")]
        public async Task IndexGet_BreadcrumbShouldBeSet(string tab)
        {
            //act
            await ManageEvidenceController.Index(tab);

            //assert
            Breadcrumb.InternalActivity.Should().Be(BreadCrumbConstant.ManageEvidenceNotesAdmin);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_AllEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var statuses = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var types = new List<NoteType>() { NoteType.Evidence };

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => statuses.SequenceEqual(g.AllowedStatuses) &&
                     types.SequenceEqual(g.NoteTypeFilterList)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_AllTransferNoteShouldBeRetrieved()
        {
            // Arrange
            var statuses = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var types = new List<NoteType>() { NoteType.Transfer };

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers");

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => statuses.SequenceEqual(g.AllowedStatuses) &&
                     types.SequenceEqual(g.NoteTypeFilterList)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]

        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_AllEvidenceNotesShouldNotBeRetrievedForInvalidDraftStatus(string tab)
        {
            // arrange
            var statusNotIncluded = NoteStatus.Draft;

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.AllowedStatuses.Contains(statusNotIncluded)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_TransferNotesShouldNotBeRetrievedForInvalidDraftStatus()
        {
            // arrange
            var statusNotIncluded = NoteStatus.Draft;

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers");

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.AllowedStatuses.Contains(statusNotIncluded)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]

        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_AllEvidenceNotesShouldNotBeRetrievedForInvalidTransferNoteType(string tab)
        {
            // arrange
            var typeFilterNotIncluded = NoteType.Transfer;

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_TransferNotesShouldNotBeRetrievedForInvalidTransferNoteType()
        {
            // arrange
            var typeFilterNotIncluded = NoteType.Evidence;

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers");

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_ViewModelShouldBeBuilt()
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers");

            //asset
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index(tab, manageEvidenceNote);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData && 
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTabWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt()
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", manageEvidenceNote);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]

        public async Task IndexGet_GivenViewAllEvidenceNotesTabWithReturnedData_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            //act
            await ManageEvidenceController.Index("view-all-evidence-notes", model);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.ManageEvidenceNoteViewModel == model))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]

        public async Task IndexGet_GivenViewAllEvidenceTransfersTabWithReturnedData_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", model);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(
                A<ViewAllEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.ManageEvidenceNoteViewModel == model))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceNotesViewModel_ViewAllEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var viewModel = TestFixture.Create<ViewAllEvidenceNotesViewModel>();

            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(A<ViewAllEvidenceNotesMapTransfer>._)).Returns(viewModel);

            //act
            var result = await ManageEvidenceController.Index(tab) as ViewResult;

            //asset
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task IndexGet_GivenViewAllTransferNotesViewModel_ViewAllEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var viewModel = TestFixture.Create<ViewAllTransferNotesViewModel>();

            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(A<ViewAllEvidenceNotesMapTransfer>._)).Returns(viewModel);

            //act
            var result = await ManageEvidenceController.Index("view-all-evidence-transfers") as ViewResult;

            //asset
            result.Model.Should().Be(viewModel);
        }

        [Theory]
        [InlineData("view-all-evidence-notes", "ViewAllEvidenceNotes")]
        [InlineData("view-all-evidence-transfers", "ViewAllTransferNotes")]
        public async void Index_GivenATabName_CorrectViewShouldBeReturned(string tab, string view)
        {
            var result = await ManageEvidenceController.Index(tab) as ViewResult;

            result.ViewName.Should().Be(view);
        }

        public static IEnumerable<object[]> ManageEvidenceModelData =>
          new List<object[]>
          {
                new object[] { null },
                new object[] { new ManageEvidenceNoteViewModel() },
          };
    }
}
