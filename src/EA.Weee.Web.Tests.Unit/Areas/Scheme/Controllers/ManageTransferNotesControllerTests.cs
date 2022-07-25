namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Controllers;
    using Xunit;

    public class ManageTransferNotesControllerTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageTransferNotesController ManageTransferNotesController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid OrganisationId;
        protected readonly Fixture Fixture;

        public ManageTransferNotesControllerTests()
        {
            Fixture = new Fixture();
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            OrganisationId = Guid.NewGuid();
            ManageTransferNotesController = new ManageTransferNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(DateTime.Now);
        }

        [Fact]
        public void CheckManageTransferNotesControllerInheritsBalancingSchemeEvidenceBaseController()
        {
            typeof(ManageTransferNotesController).BaseType.Name.Should().Be(nameof(BalancingSchemeEvidenceBaseController));
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

        [Theory]
        [InlineData(null)]
        //[InlineData("view-and-transfer-evidence")]
        //[InlineData("evidence-summary")]
        //[InlineData("outgoing-transfers")]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_BreadcrumbShouldBeSet(string tab)
        {
            //arrange
            var schemeName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(new List<EvidenceNoteData>());
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(new ReviewSubmittedManageEvidenceNotesSchemeViewModel());
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(schemeName);

            //act
            await ManageTransferNotesController.Index(organisationId, tab);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(schemeName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        //[InlineData("view-and-transfer-evidence")]
        //[InlineData("evidence-summary")]
        //[InlineData("outgoing-transfers")]
        public async Task IndexGet_CurrentSystemTimeShouldBeRetrieved(string tab)
        {
            //act
            await ManageTransferNotesController.Index(OrganisationId, tab);

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
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageTransferNotesController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(currentDate.Year) &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAndPreviouslySelectedComplianceYear_SubmittedEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var complianceYear = Fixture.Create<short>();
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            var model = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageTransferNotesController.Index(OrganisationId, tab, model);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) &&
                     status.SequenceEqual(g.AllowedStatuses) &&
                     g.ComplianceYear.Equals(complianceYear) &&
                     g.NoteTypeFilterList.SequenceEqual(noteTypes)))).MustHaveHappenedOnceExactly();
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
            await ManageTransferNotesController.Index(OrganisationId, tab);

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
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageTransferNotesController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(String.Empty) &&
                         a.CurrentDate.Equals(currentDate)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt(string tab)
        {
            // Arrange
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            var returnList = new List<EvidenceNoteData>() { evidenceData };
            var currentDate = Fixture.Create<DateTime>();
            var model = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act

            await ManageTransferNotesController.Index(OrganisationId, tab, model);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(String.Empty) &&
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
            var result = await ManageTransferNotesController.Index(OrganisationId, tab) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData("review-submitted-evidence", "ReviewSubmittedEvidence")]
        //[InlineData("view-and-transfer-evidence", "ViewAndTransferEvidence")]
        //[InlineData("evidence-summary", "ReviewSubmittedEvidence")]
        //[InlineData("outgoing-transfers", "OutgoingTransfers")]
        public async void Index_GivenATabName_CorrectViewShouldBeReturned(string tab, string view)
        {
            var pcs = Guid.NewGuid();

            var result = await ManageTransferNotesController.Index(pcs, tab) as ViewResult;

            result.ViewName.Should().Be(view);
        }
    }
}
