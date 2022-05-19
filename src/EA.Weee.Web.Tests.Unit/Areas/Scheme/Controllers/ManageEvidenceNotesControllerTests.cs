namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
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
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Helpers;
    using Core.Tests.Unit.Helpers;
    using Web.Areas.Aatf.ViewModels;
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
        private readonly short complianceYear;

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
            complianceYear = (short)DateTime.Now.Year;
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
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("transferred-out")]
        public async Task IndexGet_BreadcrumbShouldBeSet(string tab)
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(new List<EvidenceNoteData>());
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(new ReviewSubmittedManageEvidenceNotesSchemeViewModel());
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await ManageEvidenceController.Index(organisationId, tab);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("view-and-transfer-evidence")]
        [InlineData("review-submitted-evidence")]
        [InlineData("evidence-summary")]
        [InlineData("transferred-out")]
        public async Task IndexGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache(string tab)
        {
            // Arrange

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
        [InlineData("transferred-out")]
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

            //act
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) && status.SequenceEqual(g.AllowedStatuses)))).MustHaveHappenedOnceExactly();
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
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);

            //act
            
            await ManageEvidenceController.Index(OrganisationId, tab);

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReturnedData_AndReviewSubmittedAction_ReviewSubmittedEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString());

            //asset
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenReturnedData_AndViewAndTransferAction_ViewAndTransferEvidenceNotesViewModelShouldBeBuilt()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            var evidenceData = Fixture.Create<EvidenceNoteData>();
            List<EvidenceNoteData> returnList = new List<EvidenceNoteData>() { evidenceData };

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(returnList);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(
                A<ViewAndTransferEvidenceViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(OrganisationId) && a.Notes.Equals(returnList) &&
                         a.SchemeName.Equals(organisationName)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenDefaultViewModel_DefaultModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<ReviewSubmittedManageEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenReviewSubmittedEvidenceNotesViewModel_ReviewSubmittedEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<ReviewSubmittedManageEvidenceNotesSchemeViewModel>();

            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(A<ReviewSubmittedEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString()) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_EvidenceNotesShouldBeRetrieved()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });
            var status = new List<NoteStatus>()
            {
                NoteStatus.Approved,
                NoteStatus.Rejected,
                NoteStatus.Void,
                NoteStatus.Returned
            };

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) && status.SequenceEqual(g.AllowedStatuses)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_SubmittedEvidenceNoteShouldNotBeRetrievedForInvalidStatus()
        {
            // Arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>.That.Matches(
                g => g.OrganisationId.Equals(OrganisationId) && g.AllowedStatuses.Contains(NoteStatus.Submitted)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task IndexGet_GivenViewAndTransferEvidenceNotesViewModel_ViewAndTransferEvidenceNotesViewModelShouldBeReturned()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            var model = Fixture.Create<SchemeViewAndTransferManageEvidenceSchemeViewModel>();

            A.CallTo(() => Mapper.Map<SchemeViewAndTransferManageEvidenceSchemeViewModel>(A<ViewAndTransferEvidenceViewModelMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.Index(OrganisationId, ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString()) as
                    ViewResult;

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
    }
}
