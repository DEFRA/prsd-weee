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
    using Weee.Tests.Core;
    using Xunit;

    public class ManageTransferNotesControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly ManageTransferNotesController manageTransferNotesController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;

        public ManageTransferNotesControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            organisationId = Guid.NewGuid();
            manageTransferNotesController = new ManageTransferNotesController(mapper, breadcrumb, cache, () => weeeClient);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(DateTime.Now);
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
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(new ReviewSubmittedManageEvidenceNotesSchemeViewModel());
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(schemeName);

            //act
            await manageTransferNotesController.Index(organisationId, tab);

            //assert
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.ExternalOrganisation.Should().Be(schemeName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
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
            await manageTransferNotesController.Index(organisationId, tab);

            //asset
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTab_SubmittedEvidenceNoteShouldBeRetrieved(string tab)
        {
            // Arrange
            var status = new List<NoteStatus>() { NoteStatus.Submitted };
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var currentDate = TestFixture.Create<DateTime>();
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await manageTransferNotesController.Index(organisationId, tab);

            //asset
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(organisationId) &&
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
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var currentDate = TestFixture.Create<DateTime>();
            var complianceYear = TestFixture.Create<short>();
            var noteTypes = new List<NoteType>() { NoteType.Evidence, NoteType.Transfer };

            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(e => e.SelectedComplianceYear, complianceYear).Create();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await manageTransferNotesController.Index(organisationId, tab, model);

            //asset
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(organisationId) &&
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
            await manageTransferNotesController.Index(organisationId, tab);

            //asset
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.AllowedStatuses.Contains(status)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAlongWithReturnedData_ViewModelShouldBeBuilt(string tab)
        {
            // Arrange
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var currentDate = TestFixture.Create<DateTime>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await manageTransferNotesController.Index(organisationId, tab);

            //asset
            A.CallTo(() => mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(organisationId) && 
                         a.NoteData.Equals(noteData) &&
                         a.Scheme == null &&
                         a.CurrentDate.Equals(currentDate)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenDefaultAndReviewTabAlongWithReturnedDataAndManageEvidenceNoteViewModel_ViewModelShouldBeBuilt(string tab)
        {
            // Arrange
            var evidenceData = TestFixture.Create<EvidenceNoteData>();
            var currentDate = TestFixture.Create<DateTime>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetEvidenceNoteByPbsOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act

            await manageTransferNotesController.Index(organisationId, tab, model);

            //asset
            A.CallTo(() => mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(organisationId) && 
                         a.NoteData.Equals(noteData) &&
                         a.Scheme == null &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.ManageEvidenceNoteViewModel.Equals(model)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("review-submitted-evidence")]
        public async Task IndexGet_GivenReviewSubmittedEvidenceNotesViewModel_ReviewSubmittedEvidenceNotesViewModelShouldBeReturned(string tab)
        {
            //arrange
            var model = TestFixture.Create<ReviewSubmittedManageEvidenceNotesSchemeViewModel>();

            A.CallTo(() => mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await manageTransferNotesController.Index(organisationId, tab) as ViewResult;

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

            var result = await manageTransferNotesController.Index(pcs, tab) as ViewResult;

            result.ViewName.Should().Be(view);
        }
    }
}
