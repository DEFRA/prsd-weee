namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using Core.Scheme;
    using EA.Weee.Requests.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Areas.AatfEvidence.Controllers;
    using Web.Infrastructure;
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
            var result = await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId) as ViewResult;

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
            await ManageEvidenceController.CreateEvidenceNote(organisationId, AatfId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_SchemesListShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId);

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
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId);

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
            var result = await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId) as ViewResult;

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
            await ManageEvidenceController.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), organisationId, AatfId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_SchemesListShouldBeRetrieved()
        {
            //arrange
            AddModelError();

            //act
            await ManageEvidenceController.CreateEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), OrganisationId, AatfId);

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
            await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId);

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
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as ViewResult;

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
            await ManageEvidenceController.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_RequestCreatorShouldBeCalled()
        {
            //arrange
            var model = ValidModel();

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelIsValid_ApiShouldBeCalled()
        {
            //arrange
            var model = ValidModel();
            var request = Request();

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenApiHasBeenCalledAndSubmittedNoteCreated_ShouldRedirectToViewDraftEvidenceNote()
        {
            //arrange
            var model = ValidModel();
            
            var evidenceNoteId = Fixture.Create<Guid>();
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Returns(evidenceNoteId);

            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["aatfId"].Should().Be(AatfId);
            result.RouteValues["evidenceNoteId"].Should().Be(evidenceNoteId);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenApiHasBeenCalledAndDraftNoteCreated_ShouldRedirectToViewDraftEvidenceNote()
        {
            //arrange
            var model = ValidModel();

            var evidenceNoteId = Fixture.Create<Guid>();
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Returns(evidenceNoteId);

            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(AatfEvidenceRedirect.ViewEvidenceRouteName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["aatfId"].Should().Be(AatfId);
            result.RouteValues["evidenceNoteId"].Should().Be(evidenceNoteId);
        }

        [Theory]
        [InlineData(NoteStatus.Draft)]
        [InlineData(NoteStatus.Submitted)]
        public async Task CreateEvidenceNotePost_GivenApiHasBeenCalled_ViewDataShouldHaveNoteStatusAdded(NoteStatus status)
        {
            //arrange
            var model = ValidModel();
            var request = Request(status);

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(status);
        }
    }
}