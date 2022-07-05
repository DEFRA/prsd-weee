namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.Helpers;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Requests.Shared;
    using Xunit;

    public class ManageEvidenceNotesControllerTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid OrganisationId;
        protected readonly Fixture Fixture;
        protected readonly IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest> TransferNoteRequestCreator;

        public ManageEvidenceNotesControllerTests()
        {
            Fixture = new Fixture();
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            OrganisationId = Guid.NewGuid();
            TransferNoteRequestCreator = A.Fake<IRequestCreator<TransferEvidenceNoteCategoriesViewModel, TransferEvidenceNoteRequest>>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient);
        
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(DateTime.Now);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = Fixture.Create<string>() });
        }

        [Fact]
        public void ManageEvidenceNotesControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void IndexGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[]
                {
                    typeof(Guid), 
                    typeof(string),
                    typeof(ManageEvidenceNoteViewModel)
                }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferPost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Transfer", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_BreadcrumbShouldBeSet(string tab)
        {
            //arrange
            var schemeName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(new List<EvidenceNoteData>());
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(new ReviewSubmittedManageEvidenceNotesSchemeViewModel());
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(schemeName);

            //act
            await ManageEvidenceController.Index(organisationId, tab);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(schemeName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache(string tab)
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("outgoing-transfers")]
        public async Task IndexGet_CurrentSystemTimeShouldBeRetrieved(string tab)
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTab_SubmittedEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) && 
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(currentDate.Year) &&
                     g.TransferredOut == false))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAndPreviouslySelectedComplianceYear_SubmittedEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var complianceYear = Fixture.Create<short>();

            var model = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, tab, model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.TransferredOut == false))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(NoteStatus.Approved, "review-submitted-evidence")]
        [InlineData(NoteStatus.Draft, "review-submitted-evidence")]
        [InlineData(NoteStatus.Returned, "review-submitted-evidence")]
        [InlineData(NoteStatus.Void, "review-submitted-evidence")]
        [InlineData(NoteStatus.Rejected, "review-submitted-evidence")]
        [InlineData(NoteStatus.Approved, null)]
        [InlineData(NoteStatus.Draft, null)]
        [InlineData(NoteStatus.Returned, null)]
        [InlineData(NoteStatus.Void, null)]
        [InlineData(NoteStatus.Rejected, null)]
        public async Task IndexGet_GivenDefaultAndReviewTab_SubmittedEvidenceNoteShouldNotBeRetrievedForInvalidStatus(NoteStatus status, string tab)
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.AllowedStatuses.Contains(status)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAlongWithReturnedData_ViewModelShouldBeBuilt(string tab)
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName});
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(schemeName) &&
                         a.CurrentDate.Equals(currentDate)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt(string tab)
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var model = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act

            await ManageEvidenceController.Index(OrganisationId, tab, model);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(schemeName) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ManageEvidenceNoteViewModel.Equals(model)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedData_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<ViewAndTransferEvidenceViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(schemeName) &&
                         a.CurrentDate.Equals(currentDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var model = Fixture.Create<ManageEvidenceNoteViewModel>();
            
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), model);

            //asset
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<ViewAndTransferEvidenceViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && 
                         a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(schemeName) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ManageEvidenceNoteViewModel.Equals(model)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenReviewSubmittedEvidenceNotesViewModel_ReviewSubmittedEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var model = Fixture.Create<ReviewSubmittedManageEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedData_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var currentDate = Fixture.Create<DateTime>();
            var status = new List<NoteStatus>()
            {
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Void,
                NoteStatus.Returned
            };

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(currentDate.Year) &&
                     g.TransferredOut == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var currentDate = Fixture.Create<DateTime>();
            var status = new List<NoteStatus>()
            {
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Void,
                NoteStatus.Returned
            };
            var complianceYear = Fixture.Create<short>();
            var model = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString(), model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(model.SelectedComplianceYear)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_SubmittedEvidenceNoteShouldNotBeRetrievedForInvalidStatus()
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.AllowedStatuses.Contains(NoteStatus.Submitted)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange

            var model = Fixture.Create<SchemeViewAndTransferManageEvidenceSchemeViewModel>();

            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(A<ViewAndTransferEvidenceViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("view-and-transfer-evidence")]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModelWithExistingManageEvidenceNoteViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var manageEvidenceNoteViewModel = Fixture.Create<ManageEvidenceNoteViewModel>();
            var model = Fixture.Create<SchemeViewAndTransferManageEvidenceSchemeViewModel>();

            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(A<ViewAndTransferEvidenceViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, tab, manageEvidenceNoteViewModel) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("view-and-transfer-evidence", "ViewAndTransferEvidence")]
        [InlineData("review-submitted-evidence", "ReviewSubmittedEvidence")]
        [InlineData("evidence-summary", "ReviewSubmittedEvidence")]
        [InlineData("outgoing-transfers", "OutgoingTransfers")]
        public async void Index_GivenATabName_CorrectViewShouldBeReturned(string tab, string view)
        {
            var pcs = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(pcs, tab) as ViewResult;

            result.ViewName.Should().Be(view);
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTab_EvidenceNoteDataShouldBeRetrieved()
        {
            // Arrange
            var statuses = GetOutgoingTransfersAllowedStatuses();
            var noteTypes = new List<NoteType>() { NoteType.Transfer };
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers");

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     statuses.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(currentDate.Year) &&
                     g.TransferredOut == true &&
                     g.NoteTypeFilterList.SequenceEqual(g.NoteTypeFilterList)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenOutgoingTransfersTabAndPreviouslySelectedComplianceYear_SubmittedEvidenceNoteShouldBeRetrieved()
        {
            // Arrange
            var statuses = GetOutgoingTransfersAllowedStatuses();
            var noteTypes = new List<NoteType>() { NoteType.Transfer };
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var complianceYear = Fixture.Create<short>();

            var model = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     statuses.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.TransferredOut == true &&
                     g.NoteTypeFilterList.SequenceEqual(g.NoteTypeFilterList)))).MustHaveHappenedOnceExactly();
        }

        public static IEnumerable<object[]> ManageEvidenceModelData =>
            new List<object[]>
            {
                new object[] { null },
                new object[] { new ManageEvidenceNoteViewModel() },
            };

        [Theory]
        [MemberData(nameof(ManageEvidenceModelData))]

        public async Task IndexGet_GivenOutgoingTransfersTabWithReturnedData_ViewModelShouldBeBuilt(ManageEvidenceNoteViewModel model)
        {
            // Arrange
            var schemeName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(new SchemePublicInfo() { Name = schemeName });
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers", model);

            //asset
            A.CallTo(() => Mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(
                A<TransferredOutEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(schemeName) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ManageEvidenceNoteViewModel == model))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenTransferredOutEvidenceNotesSchemeViewModel_TransferredOutEvidenceNotesViewModelMapShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<TransferredOutEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<TransferredOutEvidenceNotesSchemeViewModel>(A<TransferredOutEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, "outgoing-transfers") as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public void TransferPost_PageRedirectsToTransferPage()
        {
            var result = ManageEvidenceController.Transfer(OrganisationId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("TransferEvidenceNote");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
        }

        private List<NoteStatus> GetOutgoingTransfersAllowedStatuses()
        {
            return new List<NoteStatus>
            {
                        NoteStatus.Draft,
                        NoteStatus.Approved,
                        NoteStatus.Rejected,
                        NoteStatus.Submitted,
                        NoteStatus.Void,
                        NoteStatus.Returned
            };
        }
    }
}
