namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;

    public class ManageEvidenceNotesControllerTransferEvidenceNoteTests
    {
        protected readonly IWeeeClient WeeeClient;
        protected readonly IMapper Mapper;
        protected readonly ManageEvidenceNotesController ManageEvidenceController;
        protected readonly BreadcrumbService Breadcrumb;
        protected readonly IWeeeCache Cache;
        protected readonly Guid OrganisationId;
        protected readonly Fixture Fixture;
        protected readonly IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> TransferNoteRequestCreator;

        public ManageEvidenceNotesControllerTransferEvidenceNoteTests()
        {
            WeeeClient = A.Fake<IWeeeClient>();
            Breadcrumb = A.Fake<BreadcrumbService>();
            Cache = A.Fake<IWeeeCache>();
            Mapper = A.Fake<IMapper>();
            OrganisationId = Guid.NewGuid();
            TransferNoteRequestCreator = A.Fake<IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest>>();
            ManageEvidenceController = new ManageEvidenceNotesController(Mapper, Breadcrumb, Cache, () => WeeeClient, TransferNoteRequestCreator);
            Fixture = new Fixture();
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
        public void TransferEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteDataViewModel) }).Should()
             .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteDataViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void TransferPost_PageRedirectsToTransferPage()
        {
            var result = ManageEvidenceController.Transfer(OrganisationId) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("TransferEvidenceNote");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenValidOrganisation__BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            // act
            await ManageEvidenceController.TransferEvidenceNote(OrganisationId);

            // assert
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_ModelWithDefaultValuesIsCreated()
        {
            // act
            var result = await ManageEvidenceController.TransferEvidenceNote(OrganisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));
            model.OrganisationId.Should().Be(OrganisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().BeNull();
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_CategoryValuesShouldBeRetrievedFromTheWeeCategory()
        {
            // act
            var categoryValues = new CategoryValues<CategoryBooleanViewModel>();
            var result = await ManageEvidenceController.TransferEvidenceNote(OrganisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;
       
            // assert
            model.CategoryValues.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
            for (int i = 0; i < categoryValues.Count; i++)
            {
                model.CategoryValues.ElementAt(i).Should().BeEquivalentTo(categoryValues.ElementAt(i));
            }
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesListIsNotEmpty_ModelWithSchemesIsCreated()
        {
            // arrange
            var schemeData = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);

            // act
            var result = await ManageEvidenceController.TransferEvidenceNote(OrganisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            model.SchemasToDisplay.Should().Equal(schemeData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).MustHaveHappened();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesListIsNotEmpty_ShouldCallToGetScgemes()
        {
            // arrange
            var schemeData = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);

            // act
            var result = await ManageEvidenceController.TransferEvidenceNote(OrganisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>.That
                .Matches(s => s.IncludeWithdrawn.Equals(false))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            // arrange 
            var model = new TransferEvidenceNoteDataViewModel();
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();
            model.OrganisationId = organisationId;

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            AddModelError();

            // act
            await ManageEvidenceController.TransferEvidenceNote(model);

            // asset
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_GetApprovedSchemesShouldRetrieved()
        {
            AddModelError();

            //act
            await ManageEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteDataViewModel>());

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteDataViewModel();
            AddModelError();

            //act
            var result = await ManageEvidenceController.TransferEvidenceNote(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValid_GetSchemesShouldNotBeRetrieved()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var mockSession = A.Fake<HttpSessionStateBase>();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);

            //act
            await ManageEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
            A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValid_TransferRequestCreatorShouldBeCalled()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);

            //act
            await ManageEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => TransferNoteRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenAModelIsValidAndRequestIsCreated_ShouldRedirectToAction()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);

            A.CallTo(() => TransferNoteRequestCreator.ViewModelToRequest(A<TransferEvidenceNoteDataViewModel>._)).Returns(transferRequest);

            //act
            var result = await ManageEvidenceController.TransferEvidenceNote(model) as RedirectToRouteResult;

            // assert
            /* to be updated when actual redirect happens */
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Holding");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenRequestIsCreated_SessionShouldNotBeNull()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);

            A.CallTo(() => TransferNoteRequestCreator.ViewModelToRequest(A<TransferEvidenceNoteDataViewModel>._)).Returns(transferRequest);

            //act
            await ManageEvidenceController.TransferEvidenceNote(model);

            // assert
            ManageEvidenceController.Session.Should().NotBeNull();
            ManageEvidenceController.Session.SessionID.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_SessionShouldBeEmpty()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(ManageEvidenceController);

            AddModelError();

            //act
            await ManageEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteDataViewModel>());

            // assert
            ManageEvidenceController.Session.Keys.Should().BeNull();
            ManageEvidenceController.Session.SessionID.Should().BeEmpty();
        }

        private void AddModelError()
        {
            ManageEvidenceController.ModelState.AddModelError("error", "error");
        }

        private TransferEvidenceNoteDataViewModel GetValidModel(Guid organisationId, Guid selectedScheme)
        {
            return new TransferEvidenceNoteDataViewModel { OrganisationId = organisationId, SelectedSchema = selectedScheme };
        }

        private TransferEvidenceNoteRequest GetRequest()
        {
            var listIds = Fixture.CreateMany<int>().ToList();
            return new TransferEvidenceNoteRequest(Guid.NewGuid(), listIds);
        }
    }
}
