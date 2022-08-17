namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Scheme;
    using Core.Tests.Unit.Helpers;
    using Web.Areas.Scheme.Attributes;
    using Web.Extensions;
    using Web.Filters;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ManageEvidenceNotesControllerReviewEvidenceNoteTests : SimpleUnitTestBase
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid RecipientId;
        protected readonly Guid OrganisationId;
        protected readonly Guid EvidenceNoteId;
        protected readonly ISessionService SessionService;
        private readonly ConfigurationService configurationService;

        public ManageEvidenceNotesControllerReviewEvidenceNoteTests()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            SessionService = A.Fake<ISessionService>();
            RecipientId = Guid.NewGuid();
            OrganisationId = Guid.NewGuid();
            EvidenceNoteId = Guid.NewGuid();
            configurationService = A.Fake<ConfigurationService>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, SessionService, configurationService);

            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(
                new ReviewEvidenceNoteViewModel()
                {
                    ViewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel()
                    {
                        Status = NoteStatus.Submitted
                    }
                });
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveCheckCanApproveNoteAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<CheckCanApproveNoteAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int) }).Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string) }).Should().BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(ReviewEvidenceNoteViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(ReviewEvidenceNoteViewModel) }).Should()
             .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);

            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(organisationName);

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(organisationName);

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, TestFixture.Create<Guid>());

            //asset
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNoteId_ShouldRetrieveNote()
        {
            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(EvidenceNoteId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNoteAndStatusTempData_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Approved;

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.NoteStatus.Equals(NoteStatus.Approved) &&
                                                                 v.RedirectTab == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNoteDataAndRedirectTab_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();
            var redirectTab = TestFixture.Create<string>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Approved;

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, redirectTab);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.NoteStatus.Equals(NoteStatus.Approved) &&
                                                                 v.RedirectTab.Equals(redirectTab)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenModelIsBuilt_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Build<ViewEvidenceNoteViewModel>().With(m => m.RedirectTab, string.Empty).Create();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            // assert
            result.Model.Should().Be(model);
            ((ViewEvidenceNoteViewModel)result.Model).RedirectTab.Should().BeEmpty();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenModelIsBuilt_DefaultViewShouldBeReturned()
        {
            // act
            var result = await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_ShouldReturnedReviewEvidenceNoteView()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);
            var model = GetValidModel();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            // assert
            result.ViewName.Should().Be("ReviewEvidenceNote");
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenEvidenceNoteId_ShouldRetrieveNote()
        {
            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(EvidenceNoteId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(OrganisationId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task ReviewEvidenceNoteGet_GivenMappedModelIsNotSubmittedStatus_ShouldRedirectToManageEvidenceNotes(NoteStatus status)
        {
            if (status == NoteStatus.Submitted)
            {
                return;
            }

            //arrange
            var model = TestFixture.Build<ReviewEvidenceNoteViewModel>()
                .With(m => m.ViewEvidenceNoteViewModel, TestFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.Status, status).Create()).Create();

            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(OrganisationId);
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString());
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Build<ReviewEvidenceNoteViewModel>()
                .With(r => r.ViewEvidenceNoteViewModel, new ViewEvidenceNoteViewModel()
                {
                    Status = NoteStatus.Submitted
                }).Create();

            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithPossibleValuesIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;
            var expected = new List<string> { "Approve evidence note", "Reject evidence note", "Return evidence note" };

            // assert
            Assert.Equal<IList<string>>(expected, model.PossibleValues);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenApprovedSelectedValueIsSet_ModelWithSelectedEnumValueIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            var model = result.Model as ReviewEvidenceNoteViewModel;
            model.SelectedValue = "Approve evidence note";

            // assert
            model.SelectedEnumValue.Should().Be(Core.AatfEvidence.NoteStatus.Approved);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModelAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var model = TestFixture.Create<ReviewEvidenceNoteViewModel>();
            var organisationId = model.OrganisationId;
            var organisationName = Faker.Company.Name();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => Cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // asset
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModelAndSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var model = TestFixture.Create<ReviewEvidenceNoteViewModel>();
            var organisationId = model.OrganisationId;
            var organisationName = Faker.Company.Name();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => Cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // asset
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenAModelIsValid_ShouldRedirectToDownloadEvidenceNoteAction()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            var evidenceNoteId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();
            var selectedComplianceYear = TestFixture.Create<int>();

            var model = GetValidModel();
            model.OrganisationId = organisationId;
            model.ViewEvidenceNoteViewModel.Id = evidenceNoteId;
            model.ViewEvidenceNoteViewModel.ComplianceYear = complianceYear;

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("DownloadEvidenceNote");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewEvidenceNoteViewModel.Id);
            result.RouteValues["selectedComplianceYear"].Should().Be(model.ViewEvidenceNoteViewModel.ComplianceYear);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ShouldReturnReviewEvidenceNoteView()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            var model = TestFixture.Create<ReviewEvidenceNoteViewModel>();
            AddModelError();
            EvidenceNoteData note = TestFixture.Create<EvidenceNoteData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForSchemeRequest>._)).Returns(note);
            httpContext.AttachToController(ManageEvidenceController);

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as ViewResult;

            // assert
            result.ViewName.Should().Be("ReviewEvidenceNote");
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenModelIsValid_ApiShouldBeCalled()
        {
            //arrange
            var model = TestFixture.Build<ReviewEvidenceNoteViewModel>().With(r => r.SelectedValue, "Approve evidence note").Create();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as ViewResult;

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<SetNoteStatusRequest>.That.Matches(s => s.Status.Equals(model.SelectedEnumValue) && s.NoteId.Equals(model.ViewEvidenceNoteViewModel.Id) && s.Reason.Equals(model.Reason)))).MustHaveHappenedOnceExactly();
        }

        private const string ApproveEvidenceNote = "Approve evidence note";
        private const string RejectEvidenceNote = "Reject evidence note";
        private const string ReturnEvidenceNote = "Return evidence note";

        [Theory]
        [InlineData("Approve evidence note")]
        [InlineData("Reject evidence note")]
        [InlineData("Return evidence note")]
        public async Task ReviewEvidenceNotePost_GivenApiHasBeenCalled_TempViewDataShouldHaveNoteStatusAdded(string status)
        {
            //arrange
            var model = GetValidModel();
            model.SelectedValue = status;
            //act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be((NoteUpdatedStatusEnum)model.SelectedEnumValue);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ShouldRetrieveNote()
        {
            //arrange
            AddModelError();
            var model = TestFixture.Create<ReviewEvidenceNoteViewModel>();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(model.ViewEvidenceNoteViewModel.Id))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenExistingModelAndNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteData>();
            var model = GetValidModel();
            model.ViewEvidenceNoteViewModel.ComplianceYear = TestFixture.Create<int>();
            model.ViewEvidenceNoteViewModel.SchemeId = TestFixture.Create<Guid>();
            model.ViewEvidenceNoteViewModel.Id = TestFixture.Create<Guid>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && v.SchemeId.Equals(model.ViewEvidenceNoteViewModel.SchemeId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_MappedModelShouldBeReturned()
        {
            //arrange
            var model = GetValidModel();
            AddModelError();

            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);
            
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        private void AddModelError()
        {
            ManageEvidenceController.ModelState.AddModelError("error", "error");
        }

        private ReviewEvidenceNoteViewModel GetValidModel()
        {
            return new ReviewEvidenceNoteViewModel { PossibleValues = new List<string> { "Approved" }, SelectedValue = "Approve evidence note", ViewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel() { Id = TestFixture.Create<Guid>() } };
        }
    }
}
