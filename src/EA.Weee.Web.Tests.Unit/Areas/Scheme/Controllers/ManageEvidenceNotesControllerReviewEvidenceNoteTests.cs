namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
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
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class ManageEvidenceNotesControllerReviewEvidenceNoteTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid RecipientId;
        protected readonly Guid OrganisationId;
        protected readonly Guid EvidenceNoteId;
        protected readonly Fixture Fixture;

        public ManageEvidenceNotesControllerReviewEvidenceNoteTests()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            RecipientId = Guid.NewGuid();
            OrganisationId = Guid.NewGuid();
            EvidenceNoteId = Guid.NewGuid();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient);
            Fixture = new Fixture();
        }

        [Fact]
        public void ReviewEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void DownloadEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("DownloadEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
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
        public async Task DownloadEvidenceGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var organisationId = Fixture.Create<Guid>();
            
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await ManageEvidenceController.DownloadEvidenceNote(organisationId, EvidenceNoteId, Fixture.Create<int>());

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNoteId_ShouldRetrieveNote()
        {
            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>());

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(EvidenceNoteId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteData>();
            var complianceYear = Fixture.Create<int>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, complianceYear);

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId) && 
                                                                 v.SelectedComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenEvidenceNoteAndStatusTempData_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Approved;

            // act
            await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>());

            // assert
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) && 
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.NoteStatus.Equals(NoteStatus.Approved)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenModelIsBuilt_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<ViewEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task DownloadEvidenceGet_GivenModelIsBuilt_DefaultViewShouldBeReturned()
        {
            // act
            var result = await ManageEvidenceController.DownloadEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            // assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task DownloadEvidenceNoteGet_ShouldReturnModel()
        {
            //arrange

            //act
            var result = await ManageEvidenceController.DownloadEvidenceNote(RecipientId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            // assert
            result.Model.GetType().BaseType.Should().Be(typeof(ViewEvidenceNoteViewModel));
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_ShouldReturnedReviewEvidenceNoteView()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);
            var model = GetValidModel();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            // assert
            result.ViewName.Should().Be("ReviewEvidenceNote");
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenValidOrganisation_and_GivenValidNoteId_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>());

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenEvidenceNoteId_ShouldRetrieveNote()
        {
            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>());

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId.Equals(EvidenceNoteId))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenNote_ModelMapperShouldBeCalled()
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteData>();
            var complianceYear = Fixture.Create<int>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            // act
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, complianceYear);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(OrganisationId) &&
                                                                 v.SelectedComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<ReviewEvidenceNoteViewModel>();
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithPossibleValuesIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;
            var expected = new List<string> { "Approve evidence note", "Reject evidence note", "Return evidence note" };

            // assert
            Assert.Equal<IList<string>>(expected, model.PossibleValues);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithEmptySelectedValueIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;

            // assert
            model.SelectedValue.Should().Be(string.Empty);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenApprovedSelectedValueIsSet_ModelWithSelectedEnumValueIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId, Fixture.Create<int>()) as ViewResult;

            var model = result.Model as ReviewEvidenceNoteViewModel;
            model.SelectedValue = "Approve evidence note";

            // assert
            model.SelectedEnumValue.Should().Be(Core.AatfEvidence.NoteStatus.Approved);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            // arrange 
            var model = Fixture.Create<ReviewEvidenceNoteViewModel>();
            var organisationId = model.ViewEvidenceNoteViewModel.OrganisationId;
            var organisationName = Faker.Company.Name();
            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            EvidenceNoteData note = Fixture.Create<EvidenceNoteData>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForSchemeRequest>._)).Returns(note);
            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // asset
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenAModelIsValid_ShouldRedirectToDownloadEvidenceNoteAction()
        {
            //arrange
            var organisationId = Fixture.Create<Guid>();
            var evidenceNoteId = Fixture.Create<Guid>();
            var complianceYear = Fixture.Create<int>();
            var model = GetValidModel();
            model.ViewEvidenceNoteViewModel.OrganisationId = organisationId;
            model.ViewEvidenceNoteViewModel.Id = evidenceNoteId;
            model.ViewEvidenceNoteViewModel.SelectedComplianceYear = complianceYear;

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("DownloadEvidenceNote");
            result.RouteValues["organisationId"].Should().Be(model.ViewEvidenceNoteViewModel.OrganisationId);
            result.RouteValues["evidenceNoteId"].Should().Be(model.ViewEvidenceNoteViewModel.Id);
            result.RouteValues["selectedComplianceYear"].Should().Be(model.ViewEvidenceNoteViewModel.SelectedComplianceYear);
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ShouldReturnReviewEvidenceNoteView()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            var model = Fixture.Create<ReviewEvidenceNoteViewModel>();
            AddModelError();
            EvidenceNoteData note = Fixture.Create<EvidenceNoteData>();
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
            var model = Fixture.Build<ReviewEvidenceNoteViewModel>().With(r => r.SelectedValue, "Approve evidence note").Create();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as ViewResult;

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<SetNoteStatus>.That.Matches(s => s.Status.Equals(model.SelectedEnumValue) && s.NoteId.Equals(model.ViewEvidenceNoteViewModel.Id) && s.Reason.Equals(model.Reason)))).MustHaveHappenedOnceExactly();
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
            var model = Fixture.Create<ReviewEvidenceNoteViewModel>();

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
            var noteData = Fixture.Create<EvidenceNoteData>();
            var model = GetValidModel();
            model.ViewEvidenceNoteViewModel.SelectedComplianceYear = Fixture.Create<int>();
            model.ViewEvidenceNoteViewModel.SchemeId = Fixture.Create<Guid>();
            model.ViewEvidenceNoteViewModel.Id = Fixture.Create<Guid>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            AddModelError();

            // act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            // assert
            A.CallTo(() => Mapper.Map<ReviewEvidenceNoteViewModel>(
                A<ViewEvidenceNoteMapTransfer>.That.Matches(v => v.EvidenceNoteData.Equals(noteData) &&
                                                                 v.SchemeId.Equals(model.ViewEvidenceNoteViewModel.SchemeId) &&
                                                                 v.SelectedComplianceYear.Equals(model.ViewEvidenceNoteViewModel.SelectedComplianceYear)))).MustHaveHappenedOnceExactly();
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
            return new ReviewEvidenceNoteViewModel { PossibleValues = new List<string> { "Approved" }, SelectedValue = "Approve evidence note", ViewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel() { Id = Fixture.Create<Guid>() } };
        }
    }
}
