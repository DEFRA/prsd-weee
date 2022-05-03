namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Requests.Note;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
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
            typeof(ManageEvidenceNotesController).GetMethod("ReviewEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
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
        public async Task DownloadEvidenceNoteGet_GivenAModelIsValid_ShouldReturnModel()
        {
            //arrange
            ReviewEvidenceNoteViewModel model = GetValidModel();

            //act
            var result = await ManageEvidenceController.DownloadEvidenceNote(RecipientId, EvidenceNoteId) as ViewResult;

            // assert
            result.Model.GetType().BaseType.Should().Be(typeof(ViewEvidenceNoteViewModel));
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenAModelIsValid_ShouldRedirectToReviewEvidenceNoteAction()
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
        public async Task ReviewEvidenceNoteGet_GivenAModelIsInvalid_ShouldRedirectToReviewEvidenceNoteAction()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);
            var model = GetValidModel();
            AddModelError();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

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
            await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithDefaultValuesIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;

            // assert
            model.ViewEvidenceNoteViewModel.Should().Be(model.ViewEvidenceNoteViewModel);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithPossibleValuesIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;
            var expected = new List<string> { "Approve evidence note" };

            // assert
            Assert.Equal<IList<string>>(expected, model.PossibleValues);
        }

        [Fact]
        public async Task ReviewEvidenceNoteGet_GivenANewModelIsCreated_ModelWithEmptySelectedValueIsCreated()
        {
            // act
            var result = await ManageEvidenceController.ReviewEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;
            var model = result.Model as ReviewEvidenceNoteViewModel;

            // assert
            model.SelectedValue.Should().Be(string.Empty);
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
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
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

            //assert
            result.Model.Should().NotBeNull();
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenAModelIsValid_ShouldRedirectToDownloadEvidenceNoteAction()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);
            var model = GetValidModel();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("DownloadEvidenceNote");
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenInvalidModel_ShouldRedirectToReviewEvidenceNoteAction()
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
            var model = Fixture.Create<ReviewEvidenceNoteViewModel>();

            //act
            var result = await ManageEvidenceController.ReviewEvidenceNote(model) as ViewResult;

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<SetNoteStatus>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReviewEvidenceNotePost_GivenApiHasBeenCalled_TempViewDataShouldHaveNoteStatusAdded()
        {
            //arrange
            var model = GetValidModel();

            //act
            await ManageEvidenceController.ReviewEvidenceNote(model);

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(NoteStatus.Approved);
        }

        private void AddModelError()
        {
            ManageEvidenceController.ModelState.AddModelError("error", "error");
        }

        private ReviewEvidenceNoteViewModel GetValidModel()
        {
            return new ReviewEvidenceNoteViewModel { PossibleValues = new List<string> { "Approved" }, SelectedValue = "Approve evidence note", ViewEvidenceNoteViewModel = new ViewEvidenceNoteViewModel() };
        }
    }
}
