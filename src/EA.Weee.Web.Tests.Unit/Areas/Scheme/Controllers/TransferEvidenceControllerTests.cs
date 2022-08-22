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
    using Prsd.Core.Mediator;
    using Web.Areas.Scheme.Attributes;
    using Web.Areas.Scheme.Mappings.ToViewModels;
    using Web.Areas.Scheme.Requests;
    using Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Web.Filters;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Shared;
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
        public void TransferEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid), typeof(int) }).Should()
             .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferredEvidenceGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferredEvidence", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int) }).Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferredEvidenceGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferredEvidence", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(int) }).Should().BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void TransferEvidenceNoteGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void TransferEvidenceNoteGet_ShouldHaveCheckCanCreateTransferNoteAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferEvidenceNote", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<CheckCanCreateTransferNoteAttribute>();
        }

        [Fact]
        public void TransferTonnageGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(Guid), typeof(int), typeof(bool) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void TransferTonnageGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(Guid), typeof(int), typeof(bool) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferTonnageGet_ShouldHaveCheckCanCreateTransferNoteAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferTonnage", new[] { typeof(Guid), typeof(int), typeof(bool) }).Should().BeDecoratedWith<CheckCanCreateTransferNoteAttribute>();
        }

        [Fact]
        public void TransferFromGet_ShouldHaveHttpGetAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void TransferFromGet_ShouldHaveNoCacheFilterAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public void TransferFromGet_ShouldHaveCheckCanCreateTransferNoteAttribute()
        {
            typeof(TransferEvidenceController).GetMethod("TransferFrom", new[] { typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<CheckCanCreateTransferNoteAttribute>();
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
        public async Task TransferEvidenceNoteGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact] 
        public async Task TransferEvidenceNoteGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var organisationId = TestFixture.Create<Guid>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var organisationId = TestFixture.Create<Guid>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_ModelWithDefaultValuesIsCreated()
        {
            // act
            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);
            var complianceYear = TestFixture.Create<int>();

            var result = await transferEvidenceController.TransferEvidenceNote(organisationId, complianceYear) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));
            model.OrganisationId.Should().Be(organisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().BeNull();
            model.HasSelectedAtLeastOneCategory.Should().BeFalse();
            model.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenANewModelIsCreated_CategoryValuesShouldBeRetrievedFromTheWeeCategory()
        {
            // act
            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            var categoryValues = new CategoryValues<CategoryBooleanViewModel>();
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>()) as ViewResult;
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
            var schemeData = new List<OrganisationSchemeData>()
            {
                TestFixture.Build<OrganisationSchemeData>().With(s => s.Id, organisationId).Create(),
                TestFixture.Create<OrganisationSchemeData>(),
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemeData);

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>()) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.SchemasToDisplay.Should().Equal(schemeData.Where(s => s.Id != organisationId));
            model.SchemasToDisplay.Should().NotContain(s => s.Id.Equals(organisationId));
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenSchemesListIsNotEmpty_ShouldCallToGetSchemes()
        {
            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>());

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>.That
                .Matches(s => s.IncludePBS.Equals(false))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_TransferNoteSessionObjectShouldBeRetrieved()
        {
            // act
            await transferEvidenceController.TransferEvidenceNote(organisationId, TestFixture.Create<int>());

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
            var complianceYear = TestFixture.Create<int>();

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId, complianceYear) as ViewResult;
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
            model.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public async Task TransferEvidenceNoteGet_GivenTransferNoteSessionObjectIsRetrieved_ShouldReflectTheModel()
        {
            // arrange
            var schemeId = TestFixture.Create<Guid>();
            var request = GetRequestWithCategoryIds();
            request.RecipientId = schemeId;
            A.CallTo(() =>
             sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                 SessionKeyConstant.TransferNoteKey)).Returns(request);
            var complianceYear = TestFixture.Create<int>();
            // act
            var result = await transferEvidenceController.TransferEvidenceNote(organisationId, complianceYear) as ViewResult;
            var model = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            model.CategoryValues.Should().AllBeOfType(typeof(CategoryBooleanViewModel));

            model.CategoryBooleanViewModels.Where(c => request.CategoryIds.Contains(c.CategoryId)).Should().AllSatisfy(s =>
            {
                s.Selected.Should().Be(true);
            });

            model.OrganisationId.Should().Be(organisationId);
            model.SchemasToDisplay.Should().BeEmpty();
            model.SelectedSchema.Should().Be(schemeId);
            model.HasSelectedAtLeastOneCategory.Should().BeTrue();
            model.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModel_SchemeShouldBeRetrievedFromCache()
        {
            // arrange 
            var model = new TransferEvidenceNoteCategoriesViewModel();
            AddModelError();

            // act
            await transferEvidenceController.TransferEvidenceNote(model);

            // asset
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact] 
        public async Task TransferEvidenceNotePost_GivenInvalidModelAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            var model = new TransferEvidenceNoteCategoriesViewModel();
            var organisationName = Faker.Company.Name();
            model.OrganisationId = organisationId;

            A.CallTo(() => cache.FetchSchemePublicInfo(model.OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(model.OrganisationId)).Returns(organisationName);
            AddModelError();

            // act
            await transferEvidenceController.TransferEvidenceNote(model);

            // asset
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModelAndSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            var model = new TransferEvidenceNoteCategoriesViewModel();
            var organisationName = Faker.Company.Name();
            model.OrganisationId = organisationId;

            A.CallTo(() => cache.FetchSchemePublicInfo(model.OrganisationId)).Returns(schemeInfo);
            A.CallTo(() => cache.FetchOrganisationName(model.OrganisationId)).Returns(organisationName);
            AddModelError();

            // act
            await transferEvidenceController.TransferEvidenceNote(model);

            // asset
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
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
                A<GetOrganisationScheme>.That.Matches(sh => sh.IncludePBS.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenInvalidModelAndGivenSchemesList_ModelSchemesShouldBeSet()
        {
            var model = new TransferEvidenceNoteCategoriesViewModel()
            {
                OrganisationId = organisationId
            };
            var schemeData = new List<OrganisationSchemeData>()
            {
                TestFixture.Build<OrganisationSchemeData>().With(s => s.Id, organisationId).Create(),
                TestFixture.Create<OrganisationSchemeData>(),
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemeData);
            AddModelError();

            // act
            var result = await transferEvidenceController.TransferEvidenceNote(model) as ViewResult;
            var updatedModel = result.Model as TransferEvidenceNoteCategoriesViewModel;

            // assert
            updatedModel.SchemasToDisplay.Should().Equal(schemeData.Where(s => s.Id != organisationId));
            updatedModel.SchemasToDisplay.Should().NotContain(s => s.Id.Equals(organisationId));
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

            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
            A<GetSchemesExternal>.That.Matches(sh => sh.IncludeWithdrawn.Equals(false)))).MustNotHaveHappened();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValid_TransferNoteSessionObjectIsCalled()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);
           
            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
               SessionKeyConstant.TransferNoteKey)).Returns(null);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValidAndTransferObjectIsNull_TransferRequestCreatorShouldBeCalled()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(model, null)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValidAndTransferObjectIsNotNull_TransferRequestCreatorShouldBeCalled()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);
            var transferRequest = TestFixture.Create<TransferEvidenceNoteRequest>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(transferRequest);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(model, transferRequest)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenModelIsValidAndTransferObjectIsNotNull_SessionTransferNoteObjectShouldBeUpdatedWithSelectedNotes()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            model.PcsId = Guid.NewGuid();
            var request = GetRequestWithCategoryIds();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                SessionKeyConstant.TransferNoteKey)).Returns(null);

            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>._, A<TransferEvidenceNoteRequest>._)).Returns(request);

            //act
            await transferEvidenceController.TransferEvidenceNote(model);

            //assert
            A.CallTo(() =>
            sessionService.SetTransferSessionObject(transferEvidenceController.Session,
                A<object>.That.Matches(a => ((TransferEvidenceNoteRequest)a).OrganisationId.Equals(request.OrganisationId) &&
                                            ((TransferEvidenceNoteRequest)a).CategoryIds.Equals(request.CategoryIds) &&
                                            ((TransferEvidenceNoteRequest)a).RecipientId.Equals(request.RecipientId) &&
                                            ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.Count > 0 &&
                                            ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.TrueForAll(s => request.EvidenceNoteIds.Contains(s))),
                SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferEvidenceNotePost_GivenAModelIsValidAndRequestIsCreated_ShouldRedirectToAction()
        {
            //arrange
            var model = GetValidModel(Guid.NewGuid(), Guid.NewGuid());
            var transferRequest = GetRequest();
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(transferEvidenceController);

            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);
            
            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>._, A<TransferEvidenceNoteRequest>._)).Returns(transferRequest);

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

            A.CallTo(() =>
            sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
            SessionKeyConstant.TransferNoteKey)).Returns(null);

            A.CallTo(() => transferNoteRequestCreator.SelectCategoriesToRequest(A<TransferEvidenceNoteCategoriesViewModel>._, A<TransferEvidenceNoteRequest>._)).Returns(transferRequest);

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
        public async Task TransferFromGet_GivenOrganisationId_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenValidOrganisationAndSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            
            // act
            await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferFromGet_GivenValidOrganisationAndSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferFromGet_TransferNoteSessionObjectShouldBeRetrieved()
        {
            // act
            await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>());

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
            var complianceYear = TestFixture.Create<int>();

            // act
            var result = await transferEvidenceController.TransferFrom(organisationId, complianceYear) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["selectedComplianceYear"].Should().Be(complianceYear);
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObject_TransferNotesShouldBeRetrieved()
        {
            //arrange
            var request = GetRequest();
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            var complianceYear = TestFixture.Create<int>();

            // act
            await transferEvidenceController.TransferFrom(organisationId, complianceYear);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                        g.Categories.Equals(request.CategoryIds) && 
                        g.OrganisationId.Equals(organisationId) && 
                        g.EvidenceNotes.Count.Equals(0) &&
                        g.ComplianceYear == complianceYear)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectAndNotes_ModelShouldBeMapped()
        {
            //arrange
            var request = GetRequest();
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId, complianceYear);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(organisationId) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request) &&
                    t.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenTransferNoteSessionObjectWithEvidenceNoteIds_ModelShouldBeMapped()
        {
            var request = GetRequest();
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId, complianceYear);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.SessionEvidenceNotesId.SequenceEqual(request.EvidenceNoteIds) &&
                    t.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
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
 
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            // act
            await transferEvidenceController.TransferFrom(organisationId, complianceYear);

            // assert

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.SessionEvidenceNotesId.SequenceEqual(request.EvidenceNoteIds) &&
                    t.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromGet_GivenMappedViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();

            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceNotesViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>._)).Returns(model);

            // act
            var result = await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>()) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task TransferFromGet_TransferFromViewShouldBeReturned()
        {
            // act
            var result = await transferEvidenceController.TransferFrom(organisationId, TestFixture.Create<int>()) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferFrom");
        }

        [Fact]
        public async Task TransferFromPost_GivenInvalidModel_SchemeShouldBeRetrievedFromCache()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            AddModelError();

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsNotValidAndSchemeIsNotBalancingScheme_BreadCrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsNotValidAndSchemeIsBalancingScheme_BreadCrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            var model = TestFixture.Create<TransferEvidenceNotesViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferFrom(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
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
                                                ((TransferEvidenceNoteRequest)a).RecipientId.Equals(request.RecipientId) &&
                                                ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.Count > 0 &&
                                                ((TransferEvidenceNoteRequest)a).EvidenceNoteIds.TrueForAll(s => selectedNotes.Contains(s))),
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferFromPost_GivenModelIsValid_ShouldRedirectToTransferTonnage()
        {
            // arrange 
            var complianceYear = TestFixture.Create<int>();
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.ComplianceYear, complianceYear).Create();
          
            // act
            var result = await transferEvidenceController.TransferFrom(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("TransferTonnage");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["transferAllTonnage"].Should().Be(false);
            result.RouteValues["complianceYear"].Should().Be(complianceYear);
        }

        [Fact]
        public async Task TransferFromPost_GivenModelActionIsBack_ShouldRedirectToTransferEvidenceNoteAction()
        {
            // arrange 
            var complianceYear = TestFixture.Create<int>();
            var model = TestFixture.Build<TransferEvidenceNotesViewModel>()
                .With(t => t.ComplianceYear, complianceYear)
                .With(t => t.Action, ActionEnum.Back)
                .Create();

            AddModelError();

            // act
            var result = await transferEvidenceController.TransferFrom(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("TransferEvidenceNote");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["complianceYear"].Should().Be(model.ComplianceYear);
        }

        [Fact]
        public async Task TransferFromPost_GivenModelActionIsBack_NoViewResultShouldBeReturned()
        {
            // arrange 
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.Action, ActionEnum.Back).Create();

            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as ViewResult;

            // assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferEvidenceNoteGet_SchemeShouldBeRetrievedFromCache(bool transferAllTonnage)
        {
            //act
            await transferEvidenceController.TransferTonnage(organisationId, TestFixture.Create<int>(), transferAllTonnage);

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenSchemeThatIsNotBalancingScheme_BreadcrumbShouldBeSet(bool transferAllTonnage)
        {
            // arrange 
            var organisationName = "OrganisationName";
            var complianceYear = TestFixture.Create<int>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenSchemeThatIsBalancingScheme_BreadcrumbShouldBeSet(bool transferAllTonnage)
        {
            // arrange 
            var organisationName = "OrganisationName";
            var complianceYear = TestFixture.Create<int>();
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_TransferNoteSessionObjectShouldBeRetrieved(bool transferAllTonnage)
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage);

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnageGet_ExistingTransferTonnageModelShouldBeRetrievedAndThenCleared()
        {
            // act
            await transferEvidenceController.TransferTonnage(organisationId, TestFixture.Create<int>(), TestFixture.Create<bool>());

            // assert
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(transferEvidenceController.Session,
                    SessionKeyConstant.EditTransferTonnageViewModelKey)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => sessionService.ClearTransferSessionObject(transferEvidenceController.Session, SessionKeyConstant.EditTransferTonnageViewModelKey)).MustHaveHappenedOnceExactly());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObjectIsRetrievedAndIsNull_ShouldRedirectToManageEvidenceNotes(bool transferAllTonnage)
        {
            //arrange
            var complianceYear = TestFixture.Create<int>();
            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(null);

            // act
            var result = await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["pcsId"].Should().Be(organisationId);
            result.RouteValues["tab"].Should().Be(ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString());
            result.RouteValues["area"].Should().Be("Scheme");
            result.RouteValues["selectedComplianceYear"].Should().Be(complianceYear);
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
            var complianceYear = TestFixture.Create<int>();

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage);

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetEvidenceNotesForTransferRequest>.That.Matches(g =>
                        g.Categories.Equals(request.CategoryIds) 
                        && g.OrganisationId.Equals(organisationId) 
                        && g.EvidenceNotes.Equals(request.EvidenceNoteIds) &&
                        g.ComplianceYear == complianceYear)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObjectAndNotes_ModelShouldBeMapped(bool transferAllTonnage)
        {
            //arrange
            var request = GetRequest();
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(
                        transferEvidenceController.Session, SessionKeyConstant.EditTransferTonnageViewModelKey)).Returns(null);

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferAllTonnage);

            // assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(organisationId) &&
                    t.TransferAllTonnage.Equals(transferAllTonnage) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request) &&
                    t.ExistingTransferTonnageViewModel == null &&
                    t.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnageGet_GivenTransferNoteSessionObjectAndNotesAndExistingTonnageViewModel_ModelShouldBeMapped()
        {
            //arrange
            var request = GetRequest();
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
            var complianceYear = TestFixture.Create<int>();

            A.CallTo(() =>
                sessionService.GetTransferSessionObject<TransferEvidenceNoteRequest>(transferEvidenceController.Session,
                    SessionKeyConstant.TransferNoteKey)).Returns(request);
            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<GetEvidenceNotesForTransferRequest>._)).Returns(notes);

            var existingModel = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            A.CallTo(() => sessionService.GetTransferSessionObject<TransferEvidenceTonnageViewModel>(
                transferEvidenceController.Session, SessionKeyConstant.EditTransferTonnageViewModelKey)).Returns(existingModel);
            var transferTonnage = TestFixture.Create<bool>();

            // act
            await transferEvidenceController.TransferTonnage(organisationId, complianceYear, transferTonnage);

            // assert
            A.CallTo(() => mapper.Map<TransferEvidenceNotesViewModelMapTransfer, TransferEvidenceTonnageViewModel>(
                A<TransferEvidenceNotesViewModelMapTransfer>.That.Matches(t =>
                    t.OrganisationId.Equals(organisationId) &&
                    t.TransferAllTonnage.Equals(transferTonnage) &&
                    t.Notes.Equals(notes) &&
                    t.Request.Equals(request) &&
                    t.ExistingTransferTonnageViewModel.Equals(existingModel) &&
                    t.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
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
            var result = await transferEvidenceController.TransferTonnage(organisationId, TestFixture.Create<int>(), transferAllTonnage) as ViewResult;

            // assert
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_TransferFromViewShouldBeReturned(bool transferAllTonnage)
        {
            // act
            var result = await transferEvidenceController.TransferTonnage(organisationId, TestFixture.Create<int>(), transferAllTonnage) as ViewResult;

            // assert
            result.ViewName.Should().Be("TransferTonnage");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TransferTonnageGet_SchemeShouldBeRetrievedFromCache(bool transferAllTonnage)
        {
            // act
            await transferEvidenceController.TransferTonnage(organisationId, TestFixture.Create<int>(), transferAllTonnage);

            // assert
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenInvalidModel_SchemeShouldBeRetrievedFromCache()
        {
            // arrange 
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            AddModelError();

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValidAndSchemeIsNotBalancingScheme_BreadCrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelIsNotValidAndSchemeIsBalancingScheme_BreadCrumbShouldBeSet()
        {
            // arrange 
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();
            var model = TestFixture.Create<TransferEvidenceTonnageViewModel>();
            var organisationName = "OrganisationName";
            AddModelError();

            A.CallTo(() => cache.FetchOrganisationName(model.PcsId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(model.PcsId)).Returns(schemeInfo);

            // act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(model.PcsId);
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
        public async Task TransferTonnagePost_GivenModelActionIsBack_ShouldRedirectToTransferFromAction()
        {
            // arrange 
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.Action, ActionEnum.Back).Create();

            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as RedirectToRouteResult;

            // assert
            result.RouteValues["action"].Should().Be("TransferFrom");
            result.RouteValues["controller"].Should().Be("TransferEvidence");
            result.RouteValues["pcsId"].Should().Be(model.PcsId);
            result.RouteValues["complianceYear"].Should().Be(model.ComplianceYear);
        }

        [Fact]
        public async Task TransferTonnagePost_GivenModelActionIsBack_NoApiCallShouldBeMade()
        {
            // arrange 
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(t => t.Action, ActionEnum.Back).Create();

            AddModelError();

            // act
            var result = await transferEvidenceController.TransferTonnage(model) as RedirectToRouteResult;

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest>._)).MustNotHaveHappened();
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
            var notes = new EvidenceNoteSearchDataResult(TestFixture.CreateMany<EvidenceNoteData>(3).ToList(), 3);
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

        [Theory]
        [InlineData(ActionEnum.Submit, NoteUpdatedStatusEnum.Submitted)]
        [InlineData(ActionEnum.Save, NoteUpdatedStatusEnum.Draft)]
        public async Task TransferTonnagePost_GivenValidModel_TempDataDisplayNotificationShouldBeSet(ActionEnum action, NoteUpdatedStatusEnum expectedStatus)
        {
            //arrange
            var model = TestFixture.Build<TransferEvidenceTonnageViewModel>()
                .With(m => m.Action, action)
                .Create();

            var request = TestFixture.Build<TransferEvidenceNoteRequest>()
                .With(t => t.Status, (NoteStatus)expectedStatus).Create();

            A.CallTo(() => transferNoteRequestCreator.SelectTonnageToRequest(A<TransferEvidenceNoteRequest>._, A<TransferEvidenceTonnageViewModel>._))
                .Returns(request);

            //act
            await transferEvidenceController.TransferTonnage(model);

            // assert
            transferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification].Should().Be(expectedStatus);
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
            result.RouteValues["redirectTab"].Should().Be(Web.Extensions.DisplayExtensions.ToDisplayString(ManageEvidenceNotesDisplayOptions.OutgoingTransfers));
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenEvidenceNote_TransferNoteDataShouldBeRetrieved()
        {
            //arrange
            var id = TestFixture.Create<Guid>();

            //act
            await transferEvidenceController.TransferredEvidence(TestFixture.Create<Guid>(), id, TestFixture.Create<string>());

            //assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(r => r.EvidenceNoteId.Equals(id)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_SchemeShouldBeRetrievedFromCache()
        {
            //act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>());

            //asset
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenSchemeIsNotBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenSchemeIsBalancingScheme_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.PbsManageEvidence);
            breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task TransferredEvidenceGet_CurrentDateTimeShouldBeRetrieved()
        {
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>());

            // assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2022)]
        public async Task TransferredEvidenceGet_GivenNoteData_ModelMapperShouldBeCalled(int? complianceYear)
        {
            // arrange 
            var noteData = TestFixture.Create<TransferEvidenceNoteData>();
            var currentDate = TestFixture.Create<DateTime>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(noteData);

            var redirectTab = TestFixture.Create<string>();
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), redirectTab);

            // assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.OrganisationId.Equals(organisationId) && 
                         t.TransferEvidenceNoteData.Equals(noteData) &&
                         t.DisplayNotification == null &&
                         t.RedirectTab == redirectTab &&
                         t.SystemDateTime == currentDate)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenMappedViewModel_ModelShouldBeReturned()
        {
            // arrange 
            var viewModel = TestFixture.Create<ViewTransferNoteViewModel>();

            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>._)).Returns(viewModel);

            // act
            var result = await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>()) as ViewResult;

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
            var result = await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), TestFixture.Create<string>()) as ViewResult;

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
            var redirectTab = TestFixture.Create<string>();
            var currentDate = TestFixture.Create<DateTime>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(noteData);
            transferEvidenceController.TempData[ViewDataConstant.TransferEvidenceNoteDisplayNotification] = displayNotification;
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            
            // act
            await transferEvidenceController.TransferredEvidence(organisationId, TestFixture.Create<Guid>(), redirectTab);

            // assert
            A.CallTo(() => mapper.Map<ViewTransferNoteViewModel>(A<ViewTransferNoteViewModelMapTransfer>.That.Matches(
                    t => t.OrganisationId.Equals(organisationId) &&
                         t.TransferEvidenceNoteData.Equals(noteData) &&
                         t.DisplayNotification.Equals(displayNotification) &&
                         t.RedirectTab == redirectTab &&
                         t.SystemDateTime == currentDate)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task TransferredEvidenceGet_GivenPageNumber_ShouldPopulateViewBagWithPageNumber()
        {
            // Arrange
            var pageNumber = 3;

            //act
            var result = await transferEvidenceController.TransferredEvidence(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), "view-and-transfer-evidence", pageNumber) as ViewResult;

            //assert
            Assert.Equal(pageNumber, result.ViewBag.Page);
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
