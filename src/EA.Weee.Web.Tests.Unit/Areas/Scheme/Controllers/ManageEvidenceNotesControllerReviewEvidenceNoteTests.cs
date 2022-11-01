namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using Core.Scheme;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Attributes;
    using Web.Extensions;
    using Web.Filters;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ManageEvidenceNotesControllerReviewEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        protected new readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly Guid RecipientId;
        private readonly ConfigurationService configurationService;
        private readonly Fixture testFixture;

        public ManageEvidenceNotesControllerReviewEvidenceNoteTests()
        {
            RecipientId = Guid.NewGuid();
            testFixture = new Fixture();
            configurationService = A.Fake<ConfigurationService>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, SessionService, TemplateExecutor, PdfDocumentProvider, configurationService);

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
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveCheckCanApproveNoteAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string) }).Should()
                .BeDecoratedWith<CheckCanApproveNoteAttribute>();
        }

        [Fact]
        public void ViewEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int), typeof(bool), typeof(string) }).Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ViewEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int), typeof(bool), typeof(string) }).Should().BeDecoratedWith<NoCacheFilterAttribute>();
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
        public async Task ViewEvidenceNoteGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);

            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(organisationName);

            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => Cache.FetchOrganisationName(OrganisationId)).Returns(organisationName);

            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            Breadcrumb.OrganisationId.Should().Be(OrganisationId);
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, testFixture.Create<Guid>());

            //asset
            A.CallTo(() => Cache.FetchSchemePublicInfo(OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenEvidenceNoteId_ShouldRetrieveNote()
        {
            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(EvidenceNoteId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenEvidenceNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = testFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.PrintableVersion == false &&
                                                                 v.User == null &&
                                                                 v.QueryString == null &&
                                                                 v.RedirectTab == null &&
                                                                 v.OpenedInNewTab == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenEvidenceNoteAndStatusTempData_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = testFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Approved;

            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.NoteStatus.Equals(NoteStatus.Approved) &&
                                                                 v.RedirectTab == null &&
                                                                 v.PrintableVersion == false &&
                                                                 v.User == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("redirectTab", "queryString")]
        [InlineData("redirectTab", null)]
        [InlineData(null, "queryString")]
        [InlineData(null, null)]
        public async Task ViewEvidenceNoteGet_GivenEvidenceNoteDataWithRedirectTabAndQueryString_ModelMapperShouldBeCalled(string redirectTab, string queryString)
        {
            //arrange
            var noteData = testFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Approved;

            // act
            await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId, redirectTab: redirectTab, queryString: queryString);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.NoteStatus.Equals(NoteStatus.Approved) &&
                                                                 v.RedirectTab == redirectTab &&
                                                                 v.PrintableVersion == false &&
                                                                 v.User == null &&
                                                                 v.QueryString == queryString))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenModelIsBuilt_ModelShouldBeReturned()
        {
            //arrange
            var model = testFixture.Build<ViewEvidenceNoteViewModel>().With(m => m.RedirectTab, string.Empty).Create();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            // assert
            result.Model.Should().Be(model);
            ((ViewEvidenceNoteViewModel)result.Model).RedirectTab.Should().BeEmpty();
        }

        [Fact]
        public async Task ViewEvidenceNoteGet_GivenModelIsBuilt_DefaultViewShouldBeReturned()
        {
            // act
            var result = await ManageEvidenceController.ViewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

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
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
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
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
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

        [Theory]
        [InlineData(null)]
        [InlineData("queryString")]
        public async Task ReviewEvidenceNoteGet_GivenNote_ModelMapperShouldBeCalled(string queryString)
        {
            //arrange
            var noteData = testFixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, queryString);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.PrintableVersion == false &&
                                                                 v.User == null &&
                                                                 v.QueryString == queryString))).MustHaveHappenedOnceExactly();
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
            var model = testFixture.Build<ReviewEvidenceNoteViewModel>()
                .With(m => m.ViewEvidenceNoteViewModel, testFixture.Build<ViewEvidenceNoteViewModel>().With(v => v.Status, status).Create()).Create();

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
            var model = testFixture.Build<ReviewEvidenceNoteViewModel>()
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
            var model = testFixture.Create<ReviewEvidenceNoteViewModel>();
            var organisationId = model.OrganisationId;
            var organisationName = Faker.Company.Name();
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
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
            var model = testFixture.Create<ReviewEvidenceNoteViewModel>();
            var organisationId = model.OrganisationId;
            var organisationName = Faker.Company.Name();
            var schemeInfo = testFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
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
        public async Task ReviewEvidenceNotePost_GivenAModelIsValid_ShouldRedirectToViewEvidenceNoteAction()
        {
            //arrange
            var organisationId = testFixture.Create<Guid>();
            var evidenceNoteId = testFixture.Create<Guid>();
            var complianceYear = testFixture.Create<int>();
            var selectedComplianceYear = testFixture.Create<int>();

            var model = GetValidModel();
            model.OrganisationId = organisationId;
            model.ViewEvidenceNoteViewModel.Id = evidenceNoteId;
            model.ViewEvidenceNoteViewModel.ComplianceYear = complianceYear;

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("ViewEvidenceNote");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewEvidenceNoteViewModel.Id);
            result.RouteValues["selectedComplianceYear"].Should().Be(model.ViewEvidenceNoteViewModel.ComplianceYear);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ShouldReturnReviewEvidenceNoteView()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            var model = testFixture.Create<ReviewEvidenceNoteViewModel>();
            AddModelError();
            EvidenceNoteData note = testFixture.Create<EvidenceNoteData>();
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
            var model = testFixture.Build<ReviewEvidenceNoteViewModel>().With(r => r.SelectedValue, "Approve evidence note").Create();

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
            var model = testFixture.Create<ReviewEvidenceNoteViewModel>();

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
            var noteData = testFixture.Create<EvidenceNoteData>();
            var model = GetValidModel();
            model.ViewEvidenceNoteViewModel.ComplianceYear = testFixture.Create<int>();
            model.ViewEvidenceNoteViewModel.SchemeId = testFixture.Create<Guid>();
            model.ViewEvidenceNoteViewModel.Id = testFixture.Create<Guid>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(model.ViewEvidenceNoteViewModel.SchemeId) &&
                                                                 v.PrintableVersion == false &&
                                                                 v.User == null))).MustHaveHappenedOnceExactly();
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

        [Fact]
        public async Task ReviewEvidenceNoteGet_ViewAndTransferEvidenceTab_MapperShouldCorrectlySetPageNumber()
        {
            // Arrange
            var scheme = testFixture.Create<SchemePublicInfo>();
            var noteData = testFixture.Build<EvidenceNoteSearchDataResult>().Create();
            var currentDate = testFixture.Create<DateTime>();
            var model = testFixture.Create<ManageEvidenceNoteViewModel>();
            var pageNumber = 3;

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultExternalPagingPageSize).Returns(10);
            A.CallTo(() => Cache.FetchSchemePublicInfo(A<Guid>._)).Returns(scheme);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNotesByOrganisationRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(RecipientId, "review-submitted-evidence", model, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<ReviewSubmittedManageEvidenceNotesSchemeViewModel>(
                A<SchemeTabViewModelMapTransfer>.That.Matches(
                    a => a.OrganisationId.Equals(RecipientId) &&
                         a.NoteData == noteData &&
                         a.Scheme.Equals(scheme) &&
                         a.CurrentDate.Equals(currentDate) &&
                         a.PageNumber == pageNumber &&
                         a.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenPageNumber_ViewBagShouldBePopulatedWithPageNumber()
        {
            // Arrange
            var pageNumber = 3;

            //act
            var result = await ManageEvidenceController.ViewEvidenceNote(RecipientId, testFixture.Create<Guid>(), "review-submitted-evidence", pageNumber) as ViewResult;

            //assert
            Assert.Equal(pageNumber, result.ViewBag.Page);
        }

        private new void AddModelError()
        {
            ManageEvidenceController.ModelState.AddModelError("error", "error");
        }

        private ReviewEvidenceNoteViewModel GetValidModel()
        {
            return new ReviewEvidenceNoteViewModel { PossibleValues = new List<string> { "Approved" }, SelectedValue = "Approve evidence note", ViewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel() { Id = testFixture.Create<Guid>() } };
        }
    }
}
