namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ManageEvidenceNotesControllerTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly Fixture Fixture;
        protected readonly IWeeeCache Cache;

        public ManageEvidenceNotesControllerTests()
        {
            Fixture = new Fixture();
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Mapper = A.Fake<IMapper>();
            Cache = A.Fake<IWeeeCache>();

            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient);
        }

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
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>.That.Matches(
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
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>.That.Matches(
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
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>.That.Matches(
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var manageEvidenceNote = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>._)).Returns(returnList);

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapModel>.That.Matches(
                    a => a.Notes.Equals(returnList)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var manageEvidenceNote = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>._)).Returns(returnList);

            //act
            await ManageEvidenceController.Index(tab, manageEvidenceNote);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapModel>.That.Matches(
                    a => a.Notes.Equals(returnList) && 
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]

        public async Task IndexGet_GivenViewAllEvidenceNotesTabWithReturnedData_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotes>._)).Returns(returnList);
            //act
            await ManageEvidenceController.Index("view-all-evidence-notes", model);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewAllEvidenceNotesMapModel>.That.Matches(
                    a => a.Notes.Equals(returnList) &&
                         a.ManageEvidenceNoteViewModel == model))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceNotesViewModel_ViewAllEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var viewModel = Fixture.Create<ViewAllEvidenceNotesViewModel>();

            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(A<ViewAllEvidenceNotesMapModel>._)).Returns(viewModel);

            //act
            var result = await ManageEvidenceController.Index(tab) as ViewResult;

            //asset
            result.Model.Should().Be(viewModel);
        }

        [Theory]
        [InlineData("view-all-evidence-notes", "ViewAllEvidenceNotes")]
        public async void Index_GivenATabName_CorrectViewShouldBeReturned(string tab, string view)
        {
            var pcs = Guid.NewGuid();

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
