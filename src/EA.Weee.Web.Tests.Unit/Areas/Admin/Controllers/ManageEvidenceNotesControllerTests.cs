namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Web.Filters;
    using Web.ViewModels.Shared.Mapping;
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
        public void ManageEvidenceNotesController_ShouldHaveNoCache()
        {
            typeof(ManageEvidenceNotesController).Should().BeDecoratedWith<OutputCacheAttribute>(a =>
                a.NoStore == true &&
                a.Duration == 0 &&
                a.VaryByParam.Equals("None"));
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

        [Fact]
        public void IndexGet_ShouldHaveNoCacheFilterAttribute()
        {
            // assert
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
                {
                    typeof(string),
                    typeof(ManageEvidenceNoteViewModel),
                    typeof(int)
                }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
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

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        [InlineData("view-all-evidence-transfers")]
        public async void IndexGet_GivenMappedManageEvidenceNotesViewModel_MappedEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(tab, model) as ViewResult;

            //act
            var convertedModel = result.Model as ManageEvidenceNotesViewModel;

            convertedModel.ManageEvidenceNoteViewModel.Should().Be(model);
        }

        [Fact]
        public async void IndexGet_GivenManageEvidenceNotesViewModelAndViewAllEvidenceTransfers_ManageEvidenceNoteViewModelMapperShouldBeCalledWithCorrectValues()
        {
            //arrange
            var complianceYear = TestFixture.Create<short>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYearList = new List<int> { 2019, 2020 };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", model);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                    m.AatfData == null &&
                    m.RecipientWasteStatusFilterViewModel == null &&
                    m.SubmittedDatesFilterViewModel == null &&
                    m.FilterViewModel == null &&
                    m.CurrentDate == currentDate &&
                    m.ComplianceYear == complianceYear &&
                    m.ComplianceYearList.SequenceEqual(complianceYearList))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenManageEvidenceNotesViewModelAndViewAllEvidenceNotesAndNullTabs_ManageEvidenceNoteViewModelMapperShouldBeCalledWithCorrectValues(string tab)
        {
            //arrange
            var complianceYear = TestFixture.Create<short>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYearList = new List<int> { 2019, 2020 };
            var recipientWasteStatusViewModel = TestFixture.Create<RecipientWasteStatusFilterViewModel>();
            var submittedDatesFilterViewModel = TestFixture.Create<SubmittedDatesFilterViewModel>();
  
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);
            A.CallTo(() => Mapper.Map<RecipientWasteStatusFilterViewModel>(A<RecipientWasteStatusFilterBase>._)).Returns(recipientWasteStatusViewModel);
            A.CallTo(() => Mapper.Map<SubmittedDatesFilterViewModel>(A<SubmittedDateFilterBase>._)).Returns(submittedDatesFilterViewModel);

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            //act
            await ManageEvidenceController.Index(tab, model);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                    m.AatfData == null &&
                    m.RecipientWasteStatusFilterViewModel == recipientWasteStatusViewModel &&
                    m.SubmittedDatesFilterViewModel == submittedDatesFilterViewModel &&
                    m.FilterViewModel == model.FilterViewModel &&
                    m.CurrentDate == currentDate &&
                    m.ComplianceYear == complianceYear &&
                    m.ComplianceYearList.SequenceEqual(complianceYearList))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenNullManageEvidenceNotesViewModelAndViewAllEvidenceTransfersTab_ManageEvidenceNoteViewModelMapperShouldBeCalledWithCorrectValues()
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYearList = new List<int> { 2019, 2020 };
           
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);

            //act
            await ManageEvidenceController.Index("view-all-evidence-transfers", null);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                    m.FilterViewModel == null &&
                    m.AatfData == null &&
                    m.RecipientWasteStatusFilterViewModel == null &&
                    m.SubmittedDatesFilterViewModel == null &&
                    m.CurrentDate == currentDate &&
                    m.ComplianceYear == complianceYearList.ElementAt(0) &&
                    m.ComplianceYearList.SequenceEqual(complianceYearList))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenNullManageEvidenceNotesViewModelAndViewAllEvidenceNotesOrNullTabs_ManageEvidenceNoteViewModelMapperShouldBeCalledWithCorrectValues(string tab)
        {
            //arrange
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYearList = new List<int> { 2019, 2020 };
            var recipientWasteStatusViewModel = TestFixture.Create<RecipientWasteStatusFilterViewModel>();
            var submittedDatesFilterViewModel = TestFixture.Create<SubmittedDatesFilterViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYearList);
            A.CallTo(() => Mapper.Map<RecipientWasteStatusFilterViewModel>(A<RecipientWasteStatusFilterBase>._)).Returns(recipientWasteStatusViewModel);
            A.CallTo(() => Mapper.Map<SubmittedDatesFilterViewModel>(A<SubmittedDateFilterBase>._)).Returns(submittedDatesFilterViewModel);

            //act
            await ManageEvidenceController.Index(tab, null);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m =>
                    m.FilterViewModel == null &&
                    m.AatfData == null &&
                    m.RecipientWasteStatusFilterViewModel == recipientWasteStatusViewModel &&
                    m.SubmittedDatesFilterViewModel == submittedDatesFilterViewModel &&
                    m.CurrentDate == currentDate &&
                    m.ComplianceYear == complianceYearList.ElementAt(0) &&
                    m.ComplianceYearList.SequenceEqual(complianceYearList))))
                .MustHaveHappenedOnceExactly();
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.NoteTypeFilterList.Contains(typeFilterNotIncluded) &&
                     g.PageNumber == 1 &&
                     g.PageSize == PageSize))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_GivenSearchFilterParameters_NoteShouldBeRetrieved(string tab)
        {
            // arrange
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(tab, model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.SearchRef == model.FilterViewModel.SearchRef))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_GivenSubmittedDatesFilterViewModelFilter_NoteShouldBeRetrieved(string tab)
        {
            // arrange
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(tab, model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.StartDateSubmittedFilter == model.SubmittedDatesFilterViewModel.StartDate &&
                g.EndDateSubmittedFilter == model.SubmittedDatesFilterViewModel.EndDate))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async Task IndexGet_GivenViewAllEvidenceTransfersTab_GivenRecipientWasteStatusFilterViewModelFilter_NoteShouldBeRetrieved(string tab)
        {
            // arrange
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(tab, model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllNotesInternal>.That.Matches(
                g => g.RecipientIdFilter == model.RecipientWasteStatusFilterViewModel.ReceivedId && 
                g.NoteStatusFilter == model.RecipientWasteStatusFilterViewModel.NoteStatusValue &&
                g.ObligationTypeFilter == model.RecipientWasteStatusFilterViewModel.WasteTypeValue &&
                g.SubmittedByAatfIdFilter == model.RecipientWasteStatusFilterViewModel.SubmittedBy)))
                .MustHaveHappenedOnceExactly();
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

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>.That.Matches(
                g => status.SequenceEqual(g.AllowedStatuses)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_GivenManageEvidenceNoteViewModel_OrganisationSchemeDataShouldBeRetrieved(string tab)
        {
            // arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
              .With(e => e.SelectedComplianceYear, TestFixture.Create<int>()).Create();

            // act
            await ManageEvidenceController.Index(tab, model);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationSchemeDataForFilterRequest>.That.Matches(
                g => g.ComplianceYear == model.SelectedComplianceYear &&
                     g.AatfId == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_GivenManageEvidenceNoteViewModelIsNull_OrganisationSchemeDataShouldBeRetrieved(string tab)
        {
            // arrange
            var complianceYears = new List<int>() { TestFixture.Create<int>(), TestFixture.Create<int>(), TestFixture.Create<int>() };
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYears);

            // act
            await ManageEvidenceController.Index(tab);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationSchemeDataForFilterRequest>.That.Matches(
                g => g.ComplianceYear == complianceYears[0] &&
                     g.AatfId == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_GivenManageEvidenceNoteViewModel_GetAllAatfsForComplianceYearRequest(string tab)
        {
            // arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
              .With(e => e.SelectedComplianceYear, TestFixture.Create<int>()).Create();

            // act
            await ManageEvidenceController.Index(tab, model);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllAatfsForComplianceYearRequest>.That.Matches(
                g => g.ComplianceYear == model.SelectedComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-all-evidence-notes")]
        public async void IndexGet_GivenDefaultAndViewAllEvidenceNotesTab_GivenManageEvidenceNoteViewModelIsNull_GetAllAatfsForComplianceYearRequest(string tab)
        {
            // arrange
            var complianceYears = new List<int>() { TestFixture.Create<int>(), TestFixture.Create<int>(), TestFixture.Create<int>() };
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetComplianceYearsFilter>._)).Returns(complianceYears);

            // act
            await ManageEvidenceController.Index(tab);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAllAatfsForComplianceYearRequest>.That.Matches(
                g => g.ComplianceYear == complianceYears[0]))).MustHaveHappenedOnceExactly();
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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

            //assert
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
