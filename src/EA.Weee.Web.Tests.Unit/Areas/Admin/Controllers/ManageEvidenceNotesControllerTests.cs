namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Prsd.Core;
    using Xunit;

    public class ManageEvidenceNotesControllerTests : ManageEvidenceNotesControllerTestsBase
    {
        public ManageEvidenceNotesControllerTests()
        {
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._))
                .Returns(TestFixture.CreateMany<int>().ToList());
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
                    typeof(ManageEvidenceNoteViewModel),
                    typeof(int)
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
        public async Task IndexGet_GivenPageNumber_ViewAllEvidenceNotesViewModelMapperShouldBeCalled(string tab)
        {
            const int pageNumber = 2;

            //act
            await ManageEvidenceController.Index(tab, null, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(A<ViewEvidenceNotesMapTransfer>.That
                .Matches(v => v.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenPageNumberAndViewAllEvidenceNotes_NotesShouldBeRetrieved(string tab)
        {
            const int pageNumber = 2;

            //act
            await ManageEvidenceController.Index(tab, null, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That
                .Matches(g => g.PageNumber == pageNumber &&
                              g.PageSize == PageSize))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenPageNumberAndViewAllEvidenceTransfers_NotesShouldBeRetrieved()
        {
            const int pageNumber = 2;

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", null, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That
                .Matches(g => g.PageNumber == pageNumber &&
                              g.PageSize == PageSize))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenPageNumber_ViewAllTransferNotesViewModelModelMapperShouldBeCalled()
        {
            const int pageNumber = 2;

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", null, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(A<ViewEvidenceNotesMapTransfer>.That
                .Matches(v => v.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        [InlineData("view-all-evidence-transfers")]
        public async Task IndexGet_CurrentSystemTimeShouldBeRetrieved(string tab)
        {
            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabAndComplianceYearsList_AllEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var statuses = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var types = new List<NoteType>() { NoteType.Evidence };

            var complianceYears = new List<int>() { 1, 2, 3 };
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYears);

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That
                .Matches(g => statuses.SequenceEqual(g.AllowedStatuses) && 
                              types.SequenceEqual(g.NoteTypeFilterList) && 
                              g.ComplianceYear == complianceYears.ElementAt(0) &&
                              g.PageNumber == 1 &&
                              g.PageSize == PageSize))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabAndComplianceYearsListIsEmpty_AllEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var dateTime = new DateTime(2017, 1, 1);
            SystemTime.Freeze(dateTime);
            var statuses = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var types = new List<NoteType>() { NoteType.Evidence };

            var complianceYears = new List<int>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYears);

            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That
                .Matches(g => statuses.SequenceEqual(g.AllowedStatuses) && 
                              types.SequenceEqual(g.NoteTypeFilterList) && 
                              g.ComplianceYear == dateTime.Year &&
                              g.PageNumber == 1 &&
                              g.PageSize == PageSize))).MustHaveHappenedOnceExactly();
            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabAndExistingManageEvidenceNoteModel_AllEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var statuses = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var types = new List<NoteType>() { NoteType.Evidence };

            var manageEvidenceNoteModel = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(s => s.SelectedComplianceYear, 2019).Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(new List<int>(2020));

            //act
            await ManageEvidenceController.Index(tab, manageEvidenceNoteModel);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That
                .Matches(g => statuses.SequenceEqual(g.AllowedStatuses) && 
                              types.SequenceEqual(g.NoteTypeFilterList) && 
                              g.ComplianceYear == manageEvidenceNoteModel.SelectedComplianceYear &&
                              g.PageNumber == 1 &&
                              g.PageSize == PageSize))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_GetComplianceYearsFilterShouldBeCalled(string tab)
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            //act
            await ManageEvidenceController.Index(tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>.That.Matches(
                g => status.SequenceEqual(g.AllowedStatuses)))).MustHaveHappenedOnceExactly();
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
                     types.SequenceEqual(g.NoteTypeFilterList) &&
                     g.PageSize == PageSize &&
                     g.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                g => g.AllowedStatuses.Contains(statusNotIncluded) &&
                     g.PageNumber == 1 &&
                     g.PageSize == PageSize))).MustNotHaveHappened();
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
                g => g.AllowedStatuses.Contains(statusNotIncluded) &&
                     g.PageNumber == 1 &&
                     g.PageSize == PageSize))).MustNotHaveHappened();
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
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded) &&
                     g.PageNumber == 1 &&
                     g.PageSize == PageSize))).MustNotHaveHappened();
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
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded) &&
                     g.PageNumber == 1 &&
                     g.PageSize == PageSize))).MustNotHaveHappened();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_GetComplianceYearsFilterShouldBeCalled()
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Approved, NoteStatus.Rejected, NoteStatus.Submitted, NoteStatus.Returned, NoteStatus.Void };
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers");

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>.That.Matches(
                g => status.SequenceEqual(g.AllowedStatuses)))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData && 
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) &&
                    a.PageNumber == 1))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabWithReturnedDataAndManageEvidenceNoteViewModelAndCurrentDate_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = TestFixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(date);

            //act
            await ManageEvidenceController.Index(tab, manageEvidenceNote);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) && 
                    a.CurrentDate.Equals(date) &&
                    a.PageNumber == 1))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenDefaultAndViewAllEvidenceNotesTabWithReturnedDataAndComplianceYearsList_ViewModelShouldBeBuilt(string tab)
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var complianceYearList = new List<int> { 2019, 2020 };
          
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);
          
            //act
            await ManageEvidenceController.Index(tab, manageEvidenceNote);

            //assert
            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) && 
                    a.ComplianceYearList.SequenceEqual(complianceYearList) &&
                    a.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) &&
                    a.PageNumber == 1))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTabWithReturnedDataAndManageEvidenceNoteViewModelAndCurrentDate_ViewModelShouldBeBuilt()
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = TestFixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(date);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", manageEvidenceNote);

            //asset
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                    a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) && 
                    a.CurrentDate.Equals(date) &&
                    a.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.ManageEvidenceNoteViewModel == model &&
                         a.PageNumber == 1))).MustHaveHappenedOnceExactly();
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
                A<ViewEvidenceNotesMapTransfer>.That.Matches(
                    a => a.NoteData == noteData &&
                         a.ManageEvidenceNoteViewModel == model &&
                         a.PageNumber == 1))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAllEvidenceTransfersWithReturnedDataWithComplianceYearList_ViewModelShouldBeBuilt()
        {
            // arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var manageEvidenceNote = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = TestFixture.Create<DateTime>();
            var complianceYearList = new List<int> { 2010, 2021, 2022 };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(date);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", manageEvidenceNote);

            //assert
            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(
               A<ViewEvidenceNotesMapTransfer>.That.Matches(
                   a => a.NoteData == noteData 
                        && a.ManageEvidenceNoteViewModel.Equals(manageEvidenceNote) 
                        && a.CurrentDate.Equals(date)
                   && a.ComplianceYearList.SequenceEqual(complianceYearList) &&
                        a.PageNumber == 1))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceNotesViewModel_ViewAllEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var viewModel = TestFixture.Create<ViewAllEvidenceNotesViewModel>();

            A.CallTo(() => Mapper.Map<ViewAllEvidenceNotesViewModel>(A<ViewEvidenceNotesMapTransfer>._)).Returns(viewModel);

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

            A.CallTo(() => Mapper.Map<ViewAllTransferNotesViewModel>(A<ViewEvidenceNotesMapTransfer>._)).Returns(viewModel);

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
