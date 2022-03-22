namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using Core.Scheme;
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
            A.CallTo(() => mapper.Map<CreateNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.IsEdit.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenViewModel_ViewModelShouldBeReturned()
        {
            //arrange
            var model = new CreateNoteViewModel();
            A.CallTo(() => mapper.Map<CreateNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await controller.CreateEvidenceNote(A.Dummy<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void CreateEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(CreateNoteViewModel), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await controller.CreateEvidenceNote(A.Dummy<CreateNoteViewModel>(), organisationId);

            //assert
            breadcrumb.ExternalActivity.Should().Be("TODO:fix");
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_SchemesListShouldBeRetrieved()
        {
            //act
            await controller.CreateEvidenceNote(A.Dummy<CreateNoteViewModel>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //act
            await controller.CreateEvidenceNote(A.Dummy<CreateNoteViewModel>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => mapper.Map<CreateNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.IsEdit.Equals(true)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_DefaultViewShouldBeReturned()
        {
            //act
            var result = await controller.CreateEvidenceNote(A.Dummy<CreateNoteViewModel>(), A.Dummy<Guid>()) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_ModelShouldBeReturned()
        {
            //arrange
            var model = new CreateNoteViewModel();
            A.CallTo(() => mapper.Map<CreateNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await controller.CreateEvidenceNote(model, A.Dummy<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }
    }
}