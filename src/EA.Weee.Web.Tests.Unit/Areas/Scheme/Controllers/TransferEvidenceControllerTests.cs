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
    using Core.AatfEvidence;
    using Core.Helpers;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.Requests;
    using Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly TransferEvidenceController transferEvidenceController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;
        private readonly ITransferEvidenceRequestCreator transferNoteRequestCreator;
        private readonly ISessionService sessionService;
        
        public TransferEvidenceControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            sessionService = A.Fake<ISessionService>();
            organisationId = Guid.NewGuid();
            transferNoteRequestCreator = A.Fake<ITransferEvidenceRequestCreator>();

            transferEvidenceController = new TransferEvidenceController(() => weeeClient, breadcrumb, mapper, transferNoteRequestCreator, cache, sessionService);

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(GetRequest());
        }

        [Fact]
        public void TransferEvidenceController_ActionsShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferTonnageGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(Guid), typeof(bool) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferFromGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferTonnagePost_ShouldHaveHttpPostAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(TransferEvidenceTonnageViewModel) }).Should()
             .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteCategoriesViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferFromPost_ShouldHaveHttpPostAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(TransferEvidenceNotesViewModel) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void TransferEvidenceNotePost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(TransferEvidenceNoteCategoriesViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void TransferFromPost_ShouldHaveAntiForgeryAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(TransferEvidenceNotesViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void TransferTonnage_ShouldHaveAntiForgeryAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(TransferEvidenceTonnageViewModel) }).Should()
                .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

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
            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

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
            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            var categoryValues = new CategoryValues<CategoryBooleanViewModel>();
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.CategoryBooleanViewModels.Count.Should().Be(Enum.GetNames(typeof(WeeeCategory)).Length);
            for (int i = 0; i < categoryValues.Count; i++)
            {
                model.CategoryBooleanViewModels.ElementAt(i).Should().BeEquivalentTo(categoryValues.ElementAt(i));
            }
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesList_ModelSchemesShouldBeSet()
        {
            // arrange
            var schemeData = new List<SchemeData>()
            {
                TestFixture.Build<SchemeData>().With(s => s.OrganisationId, organisationId).Create(),
                TestFixture.Create<SchemeData>(),
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.SchemasToDisplay.Should().Equal(schemeData.Where(s => s.OrganisationId != organisationId));
            model.SchemasToDisplay.Should().NotContain(s => s.OrganisationId.Equals(organisationId));
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesListIsNotEmpty_ShouldCallToGetSchemes()
        {
            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>.That
                .Matches(s => s.IncludeWithdrawn.Equals(false))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_TransferNoteSessionObjectShouldBeRetrieved()
        {
            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenTransferNoteSessionObjectIsRetrievedAndIsNull()
        {
            // arrange
            A.CallTo(() =>
             sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                 SessionKeyConstant.TransferNoteKey)).Returns(null);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));
            model.CategoryBooleanViewModels.Should().AllSatisfy(s =>
            {
                s.Selected.Should().Be(false);
            });

            model.OrganisationId.Should().Be(organisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().BeNull();
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenTransferNoteSessionObjectIsRetrieved_ShouldReflectTheModel()
        {
            // arrange
            var request = GetRequestWithCategoryIds();
            A.CallTo(() =>
             sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                 SessionKeyConstant.TransferNoteKey)).Returns(request);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));

            model.CategoryBooleanViewModels.Where(c => request.CategoryIds.Contains(c.CategoryId)).Should().AllSatisfy(s =>
            {
                s.Selected.Should().Be(true);
            });

            model.OrganisationId.Should().Be(organisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().BeNull();
            model.HasSelectedAtLeastOneCategory.Should().BeTrue();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            // arrange 
            var model = new TransferEvidenceNoteCategoriesViewModel();
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
            await transferEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteCategoriesViewModel>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModelAndGivenSchemesList_ModelSchemesShouldBeSet()
        {
            var model = new TransferEvidenceNoteCategoriesViewModel()
            {
                OrganisationId = organisationId
            };
            var schemeData = new List<SchemeData>()
            {
                TestFixture.Build<SchemeData>().With(s => s.OrganisationId, organisationId).Create(),
                TestFixture.Create<SchemeData>(),
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemeData);
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(model) as ViewResult;
            var updatedModel = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            updatedModel.SchemasToDisplay.Should().Equal(schemeData.Where(s => s.OrganisationId != organisationId));
            updatedModel.SchemasToDisplay.Should().NotContain(s => s.OrganisationId.Equals(organisationId));
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteCategoriesViewModel();
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
            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenAModelIsValidAndRequestIsCreated_ShouldRedirectToAction()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>._)).Returns(transferRequest);

            //act
            var result = await transferEvidenceController.TransferEvidenceNote(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("TransferFrom");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(model.OrganisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenRequestIsCreated_SessionShouldBeUpdated()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>._)).Returns(transferRequest);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            // assert
            A.CallTo(() =>
                    sessionService.SetTransferSessionObject(transferEvidenceController.Session, transferRequest, SessionKeyConstant.TransferNoteKey))
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
            await transferEvidenceController.TransferEvidenceNote(A.Dummy<TransferEvidenceNoteCategoriesViewModel>());

            // assert
            A.CallTo(() =>
                    sessionService.SetTransferSessionObject(transferEvidenceController.Session, A<TransferEvidenceNoteRequest>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task TransferFromGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task TransferFromGet_TransferNoteSessionObjectShouldBeRetrieved()
        {
            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectIsRetrievedAndIsNull_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(null);

            // act
            var result = await transferEvidenceController.TransferFrom(organisationId) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
            result.RouteValues["area"].Should().Be("Scheme");
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObject_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var request = GetRequest();
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);

            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                        g.Categories.Equals(request.CategoryIds) && 
                        g.OrganisationId.Equals(organisationId) && 
                        g.EvidenceNotes.Count.Equals(0))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectAndNotes_ModelShouldBeMapped()
        {
            //arrange
            var request = GetRequest();
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(organisationId) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectWithEvidenceNoteIds_ModelShouldBeMapped()
        {
            var request = GetRequest();
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.SessionEvidenceNotesId.SequenceEqual(request.EvidenceNoteIds)))).MustHaveHappenedOnceExactly();
        }

        public static IEnumerable<object[]> EvidenceNoteIds =>
         new List<object[]>
         {
                new object[] { new List<Guid>() },
                new object[] { new List<Guid> { Guid.NewGuid() } },
                new object[] { new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() } }
         };

        [Theory]
        [MemberData(nameof(EvidenceNoteIds))]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectWithListOfEvidenceIds_ModelShouldBeMapped(List<Guid> evidenceIds)
        {
            var request = GetRequest(evidenceIds);
 
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.SessionEvidenceNotesId.SequenceEqual(request.EvidenceNoteIds)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenMappedViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await transferEvidenceController.TransferFrom(organisationId) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferFromGet_TransferFromViewShouldBeReturned()
        {
            // act
            var result = await transferEvidenceController.TransferFrom(organisationId) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferFrom");
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsNotValid_BreadCrumbShouldBeSet()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsNotValid_TransferFromViewShouldBeReturned()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferFrom(model) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferFrom");
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsNotValid_ModelShouldBeReturned()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferFrom(model) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsValid_SessionTransferNoteObjectShouldBeRetrieved()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsValid_SessionTransferNoteObjectShouldBeUpdatedWithSelectedNotes()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            model.SelectedEvidenceNotePairs = new List<GenericControlPair<Guid, bool>>()
            {
                new GenericControlPair<Guid, bool>(Guid.NewGuid(), true),
                new GenericControlPair<Guid, bool>(Guid.NewGuid(), true)
            };
            var request = GetRequest();
            var selectedNotes = model.SelectedEvidenceNotePairs.Where(a => a.Value.Equals(true)).Select(b => b.Key).ToList();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            A.CallTo(() =>
                sessionService.SetTransferSessionObject(transferEvidenceController.Session, 
                    A<object>.That.Matches(a => ((TransferEvidenceNoteRequest)a).OrganisationId.Equals(model.PcsId) &&
                                                ((TransferEvidenceNoteRequest)a).CategoryIds.Equals(request.CategoryIds) &&
                                                ((TransferEvidenceNoteRequest)a).SchemeId.Equals(request.SchemeId) &&
                                                ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.Count > 0 &&
                                                ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.TrueForAll(s => selectedNotes.Contains(s))),
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsValid_ShouldRedirectToTransferTonnage()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
          
            // act
            var result = await transferEvidenceController.TransferFrom(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("TransferTonnage");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["transferAllTonnage"].Should().Be(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenOrganisation_BreadcrumbShouldBeSet(bool transferAllTonnage)
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_TransferNoteSessionObjectShouldBeRetrieved(bool transferAllTonnage)
        {
            // act
            await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObjectIsRetrievedAndIsNull_ShouldRedirectToManageEvidenceNotes(bool transferAllTonnage)
        {
            //arrange
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(null);

            // act
            var result = await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
            result.RouteValues["area"].Should().Be("Scheme");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObject_TransferNotesShouldBeRetrieved(bool transferAllTonnage)
        {
            //arrange
            var request = GetRequest();
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                        g.Categories.Equals(request.CategoryIds) 
                        && g.OrganisationId.Equals(organisationId) 
                        && g.EvidenceNotes.Equals(request.EvidenceNoteIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObjectAndNotes_ModelShouldBeMapped(bool transferAllTonnage)
        {
            //arrange
            var request = GetRequest();
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage);

            // assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(organisationId) &&
                    t.TransferAllTonnage.Equals(transferAllTonnage) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenMappedViewModel_ModelShouldBeReturned(bool transferAllTonnage)
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_TransferFromViewShouldBeReturned(bool transferAllTonnage)
        {
            // act
            var result = await transferEvidenceController.TransferTonnage(organisationId, transferAllTonnage) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferTonnage");
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_BreadCrumbShouldBeSet()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_TransferFromViewShouldBeReturned()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferTonnage");
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_ModelShouldBeReturned()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            AddModelError();

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_SessionTransferNoteObjectShouldBeRetrieved()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            AddModelError();

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var request = GetRequest();
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            AddModelError();

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                        g.Categories.Equals(request.CategoryIds)
                        && g.OrganisationId.Equals(model.PcsId)
                        && g.EvidenceNotes.Equals(request.EvidenceNoteIds))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_ModelShouldBeMapped()
        {
            //arrange
            var request = GetRequest();
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.TransferAllTonnage, false).Create();
            var notes = TestFixture.CreateMany<EvidenceNoteData>().ToList();
            AddModelError();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(model.PcsId) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request) &&
                    t.TransferAllTonnage.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValid_MappedViewModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.TransferAllTonnage, false).Create();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferTonnagePost_ModelIsNotValid_TransferFromViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferTonnage");
        }

        [Fact]
        public async Task TransferTonnagePost_GivenValidModel_SessionTransferNoteObjectShouldBeRetrieved()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();

            //act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenValidModel_RequestShouldBeCreated()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(
                transferEvidenceController.Session,
                SessionKeyConstant.TransferNoteKey)).Returns(request);

            //act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => transferNoteRequestCreator.SelectTonnageToRequest(request, model))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenValidModel_TempDataDisplayNotificationShouldBeSet()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();

            //act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            transferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should()
                .Be(true);
        }

        [Fact]
        public async Task TransferTonnagePost_GivenValidModel_ApiShouldBeCalledWithRequest()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();
            var request = TestFixture.Create<TransferEvidenceNoteRequest>();
            A.CallTo(() => transferNoteRequestCreator.SelectTonnageToRequest(A<TransferEvidenceNoteRequest>._, A<TransferEvidenceTonnageViewModel>._)).Returns(request);

            //act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenValidModel_ShouldRedirectToViewTransferEvidenceNote()
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>().Create();
            var id = TestFixture.Create<Guid>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<TransferEvidenceNoteRequest>._)).Returns(id);

            //act
            var result = await transferEvidenceController.TransferTonnage(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["action"].Should().Be("TransferredEvidence");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["evidenceNoteId"].Should().Be(id);
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenEvidenceNote_TransferNoteDataShouldBeRetrieved()
        {
            //arrange
            var id = TestFixture.Create<Guid>();

            //act
            await transferEvidenceController.TransferredEvidence(TestFixture.Create<Guid>(), id, null, TestFixture.Create<string>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId.Equals(id)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), null, TestFixture.Create<string>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task TransferredEvidenceGet_GivenNoteData_ModelMapperShouldBeCalled(int? complianceYear)
        {
            // arrange 
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(noteData);

            var redirectTab = TestFixture.Create<string>();
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), complianceYear, redirectTab);

            // assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.OrganisationId.Equals(organisationId) && 
                         t.TransferEvidenceNoteData.Equals(noteData) &&
                         t.DisplayNotification == null &&
                         t.SelectedComplianceYear == complianceYear &&
                         t.RedirectTab == redirectTab)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenMappedViewModel_ModelShouldBeReturned()
        {
            // arrange 
            var viewModel = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(viewModel);

            // act
            var result = await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), null, TestFixture.Create<string>()) as ViewResult;

            // assert
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenMappedViewModel_ViewShouldBeReturned()
        {
            // arrange 
            var viewModel = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(viewModel);

            // act
            var result = await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), null, TestFixture.Create<string>()) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferredEvidence");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferredEvidenceGet_GivenNoteDataAndDisplayNotification_ModelMapperShouldBeCalled(bool displayNotification)
        {
            // arrange 
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._))
                .Returns(noteData);
            transferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] =
                displayNotification;

            var redirectTab = TestFixture.Create<string>();
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), null, redirectTab);

            // assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.OrganisationId.Equals(organisationId) &&
                         t.TransferEvidenceNoteData.Equals(noteData) &&
                         t.DisplayNotification.Equals(displayNotification) &&
                         t.RedirectTab == redirectTab)))
                .MustHaveHappenedOnceExactly();
        }

        private void AddModelError()
        {
            transferEvidenceController.ModelState.AddModelError("error", "error");
        }

        private TransferEvidenceNoteCategoriesViewModel GetValidModel(Guid organisationId, Guid selectedScheme)
        {
            return new TransferEvidenceNoteCategoriesViewModel { OrganisationId = organisationId, SelectedSchema = selectedScheme };
        }

        private TransferEvidenceNoteRequest GetRequest(List<Guid> evidenceIds = null)
        {
            var categoryIds = TestFixture.CreateMany<int>().ToList();
          
            if (evidenceIds == null)
            {
                evidenceIds = TestFixture.CreateMany<Guid>().ToList();
            }
           
            return new TransferEvidenceNoteRequest(Guid.NewGuid(), Guid.NewGuid(), categoryIds)
            {
                EvidenceNoteIds = evidenceIds
            };
        }

        private TransferEvidenceNoteRequest GetRequestWithCategoryIds()
        {
            var categoryIds = new List<int> { 3, 5, 9, 11 };
            var evidenceNoteIds = TestFixture.CreateMany<Guid>().ToList();

            return new TransferEvidenceNoteRequest(Guid.NewGuid(), Guid.NewGuid(), categoryIds)
            {
                EvidenceNoteIds = evidenceNoteIds
            };
        }
    }
}
