namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Areas.AatfEvidence.Controllers;
    using Web.Infrastructure;
    using Web.ViewModels.Shared;
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
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(bool) }).Should()
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
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) && c.ExistingModel == null && c.AatfId.Equals(AatfId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenViewModel_ViewModelShouldBeReturned()
        {
            //arrange
            var model = new EditEvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void CreateEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(EditEvidenceNoteViewModel), typeof(Guid), typeof(Guid) }).Should()
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
            await ManageEvidenceController.CreateEvidenceNote(A.Dummy<EditEvidenceNoteViewModel>(), organisationId, AatfId);

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
            await ManageEvidenceController.CreateEvidenceNote(A.Dummy<EditEvidenceNoteViewModel>(), OrganisationId, AatfId);

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
            var model = A.Dummy<EditEvidenceNoteViewModel>();
            AddModelError();

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId);

            //assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes) 
                                                           && c.ExistingModel.Equals(model)
                                                           && c.OrganisationId.Equals(OrganisationId) 
                                                           && c.AatfId.Equals(AatfId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new EditEvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);
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
            var model = new EditEvidenceNoteViewModel()
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
            var request = Request(NoteStatus.Submitted);
            var evidenceNoteId = Fixture.Create<Guid>();

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);
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
            var request = Request(NoteStatus.Draft);
            var evidenceNoteId = Fixture.Create<Guid>();

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Returns(evidenceNoteId);

            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(AatfEvidenceRedirect.ViewDraftEvidenceRouteName);
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

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelStateErrors_ErrorsShouldBeOrdered()
        {
            //arrange
            var model = ValidModel();
            var request = Request(NoteStatus.Submitted);

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            ManageEvidenceController.ModelState.AddModelError("ProtocolValue", new Exception());
            ManageEvidenceController.ModelState.AddModelError("ReceivedId", new Exception());
            ManageEvidenceController.ModelState.AddModelError("CategoryValues2", new Exception());
            ManageEvidenceController.ModelState.AddModelError("WasteTypeValue", new Exception());
            ManageEvidenceController.ModelState.AddModelError("StartDate", new Exception());
            ManageEvidenceController.ModelState.AddModelError("EndDate", new Exception());
            ManageEvidenceController.ModelState.AddModelError("Received-auto", new Exception());
            ManageEvidenceController.ModelState.AddModelError("CategoryValues", new Exception());

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, Fixture.Create<Guid>(), Fixture.Create<Guid>());

            //assert
            ManageEvidenceController.ModelState.ElementAt(0).Key.Should().Be("StartDate");
            ManageEvidenceController.ModelState.ElementAt(1).Key.Should().Be("EndDate");
            ManageEvidenceController.ModelState.ElementAt(2).Key.Should().Be("ReceivedId");
            ManageEvidenceController.ModelState.ElementAt(3).Key.Should().Be("Received-auto");
            ManageEvidenceController.ModelState.ElementAt(4).Key.Should().Be("WasteTypeValue");
            ManageEvidenceController.ModelState.ElementAt(5).Key.Should().Be("ProtocolValue");
            ManageEvidenceController.ModelState.ElementAt(6).Key.Should().Be("CategoryValues2");
            ManageEvidenceController.ModelState.ElementAt(7).Key.Should().Be("CategoryValues");
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_Should_CallAndClearSessionService()
        {
            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, true);

            //Assert
            A.CallTo(() => SessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(A<HttpSessionStateBase>._, A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => SessionService.SetTransferSessionObject(A<HttpSessionStateBase>._, null, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenTrueReturnFromCopyPaste_Should_CallMapperWithExistingModel()
        {
            //Arrange
            var model = ValidModel();
            A.CallTo(() => SessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(ManageEvidenceController.Session, SessionKeyConstant.EditEvidenceViewModelKey)).Returns(model);

            var schemes = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, true);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>.That.Matches(x => x.AatfId == AatfId
                && x.ExistingModel == model
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenFalseReturnFromCopyPaste_Should_CallMapperWithoutExistingModel()
        {
            //Arrange
            var schemes = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, false);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>.That.Matches(x => x.AatfId == AatfId
                && x.ExistingModel == null
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenCopyPasteAction_Should_RedirectToCopyPaste()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.CopyAndPaste;

            //Act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be(EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["returnAction"].Should().Be(EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_NotGivenCopyPasteAction_ShouldNot_RedirectToCopyPaste()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.Submit;

            //Act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteValues["controller"].Should().NotBe(EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName);
            result.RouteValues["returnAction"].Should().NotBe(EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenCopyPasteAction_Should_CallSessionService()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.CopyAndPaste;

            //Act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => SessionService.SetTransferSessionObject(A<HttpSessionStateBase>._, model, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_NotGivenCopyPasteAction_ShouldNot_CallSessionService()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.Submit;

            //Act
            var result = await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => SessionService.SetTransferSessionObject(A<HttpSessionStateBase>._, model, A<string>._)).MustNotHaveHappened();
        }
    }
}