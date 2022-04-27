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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Xunit;

    public class TransferEvidenceControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly TransferEvidenceController transferEvidenceController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;
        private readonly Fixture fixture;
        private readonly IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest> transferNoteRequestCreator;
        private readonly ISessionService sessionService;

        public TransferEvidenceControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            organisationId = Guid.NewGuid();
            transferNoteRequestCreator = A.Fake<IRequestCreator<TransferEvidenceNoteDataViewModel, TransferEvidenceNoteRequest>>();
            transferEvidenceController = new TransferEvidenceController(() => weeeClient, breadcrumb, mapper, transferNoteRequestCreator, cache, sessionService);
            fixture = new Fixture();
        }

        [Fact]
        public void TransferEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteDataViewModel) }).Should()
             .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteDataViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenValidOrganisation__BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_ModelWithDefaultValuesIsCreated()
        {
            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));
            model.OrganisationId.Should().Be(organisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().BeNull();
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_CategoryValuesShouldBeRetrievedFromTheWeeCategory()
        {
            // act
            var categoryValues = new CategoryValues<CategoryBooleanViewModel>();
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
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
            var schemeData = fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            model.SchemasToDisplay.Should().Equal(schemeData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).MustHaveHappened();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesListIsNotEmpty_ShouldCallToGetSchemes()
        {
            // arrange
            var schemeData = fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteDataViewModel;

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>.That
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

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            AddModelError();

            // act
            await transferEvidenceController.TransferEvidenceNote(model);

            // asset
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_GetApprovedSchemesShouldRetrieved()
        {
            AddModelError();

            //act
            await transferEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteDataViewModel>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteDataViewModel();
            AddModelError();

            //act
            var result = await transferEvidenceController.TransferEvidenceNote(model) as ViewResult;

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
            httpContext.AttachToController(transferEvidenceController);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
            A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValid_TransferRequestCreatorShouldBeCalled()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => transferNoteRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenAModelIsValidAndRequestIsCreated_ShouldRedirectToAction()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() => transferNoteRequestCreator.ViewModelToRequest(A<TransferEvidenceNoteDataViewModel>._)).Returns(transferRequest);

            //act
            var result = await transferEvidenceController.TransferEvidenceNote(model) as RedirectToRouteResult;

            // assert
            /* to be updated when actual redirect happens */
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("Holding");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenRequestIsCreated_SessionShouldBeUpdated()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() => transferNoteRequestCreator.ViewModelToRequest(A<TransferEvidenceNoteDataViewModel>._)).Returns(transferRequest);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            // assert
            A.CallTo(() =>
                    sessionService.SetTransferNoteSessionObject(transferEvidenceController.Session, transferRequest))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_SessionShouldNotBeEmpty()
        {
            //arrange
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            AddModelError();

            //act
            await transferEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteDataViewModel>());

            // assert
            A.CallTo(() =>
                    sessionService.SetTransferNoteSessionObject(transferEvidenceController.Session, A<TransferEvidenceNoteRequest>._)).MustNotHaveHappened();
        }

        private void AddModelError()
        {
            transferEvidenceController.ModelState.AddModelError("error", "error");
        }

        private TransferEvidenceNoteDataViewModel GetValidModel(Guid organisationId, Guid selectedScheme)
        {
            return new TransferEvidenceNoteDataViewModel { OrganisationId = organisationId, SelectedSchema = selectedScheme };
        }

        private TransferEvidenceNoteRequest GetRequest()
        {
            var listIds = fixture.CreateMany<int>().ToList();
            return new TransferEvidenceNoteRequest(Guid.NewGuid(), listIds);
        }
    }
}
