namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.Scheme;
    using EA.Weee.Requests.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Areas.AatfEvidence.Controllers;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Xunit;

    public class ManageEvidenceNotesControllerCreateEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ManageEvidenceNotesControllerInheritsExternalSiteController()
        {
            typeof(ManageEvidenceNotesController).BaseType.Name.Should().Be(nameof(AatfEvidenceBaseController));
        }

        [Fact]
        public void CreateEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await Controller.CreateEvidenceNote(OrganisationId, AatfId) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await Controller.CreateEvidenceNote(organisationId, AatfId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be("TODO:fix");
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_SchemesListShouldBeRetrieved()
        {
            //act
            await Controller.CreateEvidenceNote(OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //act
            await Controller.CreateEvidenceNote(OrganisationId, AatfId);

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.ExistingModel == null && c.AatfId.Equals(AatfId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenViewModel_ViewModelShouldBeReturned()
        {
            //arrange
            var model = new EvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await Controller.CreateEvidenceNote(OrganisationId, AatfId) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void CreateEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(EvidenceNoteViewModel), typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            AddModelError();

            //act
            await Controller.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), organisationId, AatfId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be("TODO:fix");
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_SchemesListShouldBeRetrieved()
        {
            //arrange
            AddModelError();

            //act
            await Controller.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);
            var model = A.Dummy<EvidenceNoteViewModel>();
            AddModelError();

            //act
            await Controller.CreateEvidenceNote(model, OrganisationId, AatfId);

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) 
                                                           && c.ExistingModel.Equals(model)
                                                           && c.OrganisationId.Equals(OrganisationId) 
                                                           && c.AatfId.Equals(AatfId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new EvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);
            AddModelError();

            //act
            var result = await Controller.CreateEvidenceNote(model, OrganisationId, AatfId) as ViewResult;

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
            await Controller.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_RequestCreatorShouldBeCalled()
        {
            //arrange
            var model = ValidModel();

            //act
            await Controller.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_ApiShouldBeCalled()
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

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_ApiShouldBeCalled()
        {
            //arrange
            var model = ValidModel();
            var request = Request();

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await Controller.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => WeeeClient.SendAsync<int>(A<string>._, request)).MustHaveHappenedOnceExactly();
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