namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using Core.Scheme;
    using EA.Weee.Requests.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Services;
    using Services.Caching;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Areas.AatfEvidence.Controllers;
    using Weee.Requests.Scheme;
    using Xunit;

    public class ManageEvidenceNotesControllerCreateEvidenceNoteTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly ManageEvidenceNotesController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Fixture fixture;

        public ManageEvidenceNotesControllerCreateEvidenceNoteTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            controller = new ManageEvidenceNotesController(mapper, breadcrumb, cache, () => weeeClient);
        }

        [Fact]
        public void ManageEvidenceNotesControllerInheritsExternalSiteController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }

        [Fact]
        public void CreateEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await controller.CreateEvidenceNote(A.Dummy<Guid>()) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await controller.CreateEvidenceNote(organisationId);

            //assert
            breadcrumb.ExternalActivity.Should().Be("TODO:fix");
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_SchemesListShouldBeRetrieved()
        {
            //act
            await controller.CreateEvidenceNote(A.Dummy<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //act
            await controller.CreateEvidenceNote(A.Dummy<Guid>());

            //assert
            A.CallTo(() => mapper.Map<EvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.ExistingModel == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenViewModel_ViewModelShouldBeReturned()
        {
            //arrange
            var model = new EvidenceNoteViewModel();
            A.CallTo(() => mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await controller.CreateEvidenceNote(A.Dummy<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void CreateEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(EvidenceNoteViewModel), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            AddModelError();

            //act
            await controller.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), organisationId);

            //assert
            breadcrumb.ExternalActivity.Should().Be("TODO:fix");
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_SchemesListShouldBeRetrieved()
        {
            //arrange
            AddModelError();

            //act
            await controller.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);
            var model = A.Dummy<EvidenceNoteViewModel>();
            AddModelError();

            //act
            await controller.CreateEvidenceNote(model, A.Dummy<Guid>());

            //assert
            A.CallTo(() => mapper.Map<EvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.ExistingModel.Equals(model)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new EvidenceNoteViewModel();
            A.CallTo(() => mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);
            AddModelError();

            //act
            var result = await controller.CreateEvidenceNote(model, A.Dummy<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_ModelShouldNotBeRebuilt()
        {
            //arrange
            var model = new EvidenceNoteViewModel()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ReceivedId = Guid.NewGuid()
            };
            
            //act
            await controller.CreateEvidenceNote(model, A.Dummy<Guid>());

            //assert
            A.CallTo(() => mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_ShouldBeRedirectToViewEvidenceNote()
        {
            //arrange
            var model = ValidModel();

            //act
            var result = await controller.CreateEvidenceNote(model, A.Dummy<Guid>()) as RedirectToRouteResult;

            //assert
            result.RouteValues.Should().Contain(c => c.Value.Equals("ViewDraftEvidenceNote") && c.Key.Equals("action"));
            result.RouteValues.Should().Contain(c => c.Key.Equals("evidenceNoteId"));
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";

            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be($"Manage AATF Evidence Notes");
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var model = new ManageEvidenceNoteViewModel()
            {
            };

            var result = await controller.Index(organisationId, aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexPost_ValidViewModel_PageRedirectsCreateEvidenceNote()
        {
            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = fixture.Create<Guid>(),
                AatfId = fixture.Create<Guid>(),
            };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Holding"); // TODO Change to Create Evidence Note
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            //result.RouteValues["aatfId"].Should().Be(model.SelectedId); // This will be needed
        }

        private void AddModelError()
        {
            controller.ModelState.AddModelError("error", "error");
        }

        private EvidenceNoteViewModel ValidModel()
        {
            var model = new EvidenceNoteViewModel()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ReceivedId = Guid.NewGuid()
            };
            return model;
        }
    }
}