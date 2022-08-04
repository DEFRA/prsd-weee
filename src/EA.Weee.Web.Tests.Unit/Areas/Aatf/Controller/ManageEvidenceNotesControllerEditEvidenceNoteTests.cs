namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using Core.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.ApiClient;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.ViewModels;
    using Web.Infrastructure;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Xunit;

    public class ManageEvidenceNotesControllerEditEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        private readonly EvidenceNoteData noteData;

        public ManageEvidenceNotesControllerEditEvidenceNoteTests()
        {
            noteData = Fixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(noteData);
        }

        [Fact]
        public void EditDraftEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(bool) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_SchemesListShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS.Equals(true)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await ManageEvidenceController.EditEvidenceNote(organisationId, EvidenceNoteId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_GivenEvidenceId_EvidenceNoteShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_GivenRequestData_EditEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            var schemes = Fixture.CreateMany<OrganisationSchemeData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);

            //act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<EditNoteMapTransfer>.That.Matches(
                v => v.Schemes.Equals(schemes) &&
                     v.NoteData.Equals(noteData) &&
                     v.OrganisationId.Equals(OrganisationId) &&
                     v.AatfId.Equals(noteData.AatfData.Id) && 
                     v.ExistingModel == null &&
                     v.ComplianceYear == noteData.ComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<EditEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public void EditDraftEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(EditEvidenceNoteViewModel), typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);
            AddModelError();

            //act
            await ManageEvidenceController.EditEvidenceNote(A.Dummy<EditEvidenceNoteViewModel>(), organisationId, AatfId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_SchemesListShouldBeRetrieved()
        {
            //arrange
            AddModelError();

            //act
            await ManageEvidenceController.EditEvidenceNote(A.Dummy<EditEvidenceNoteViewModel>(), OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS.Equals(true)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = Fixture.CreateMany<OrganisationSchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);
            var model = A.Dummy<EditEvidenceNoteViewModel>();
            AddModelError();

            //act
            await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId);

            //assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(
                A<EditNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes)
                                                         && c.ExistingModel.Equals(model)
                                                         && c.OrganisationId.Equals(OrganisationId)
                                                         && c.AatfId.Equals(AatfId) 
                                                         && c.NoteData == null 
                                                         && c.ComplianceYear == model.ComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new EditEvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).Returns(model);
            AddModelError();

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenModelIsValid_ModelShouldNotBeRebuilt()
        {
            //arrange
            var model = new EditEvidenceNoteViewModel()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                RecipientId = Guid.NewGuid()
            };

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenModelIsValid_RequestCreatorShouldBeCalled()
        {
            //arrange
            var model = ValidModel();

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => EditRequestCreator.ViewModelToRequest(model)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenModelIsValid_ApiShouldBeCalled()
        {
            //arrange
            var model = ValidModel();
            var request = EditRequest();

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenApiHasBeenCalledAndSubmittedNoteCreated_ShouldRedirectToViewDraftEvidenceNote()
        {
            //arrange
            var model = ValidModel();
            var request = EditRequest(NoteStatus.Submitted);
            var evidenceNoteId = Fixture.Create<Guid>();

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<EvidenceNoteBaseRequest>._)).Returns(evidenceNoteId);

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(AatfEvidenceRedirect.ViewSubmittedEvidenceRouteName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["aatfId"].Should().Be(AatfId);
            result.RouteValues["evidenceNoteId"].Should().Be(evidenceNoteId);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenApiHasBeenCalledAndDraftNoteCreated_ShouldRedirectToViewDraftEvidenceNote()
        {
            //arrange
            var model = ValidModel();
            var request = EditRequest(NoteStatus.Draft);
            var evidenceNoteId = Fixture.Create<Guid>();

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<EvidenceNoteBaseRequest>._)).Returns(evidenceNoteId);

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteName.Should().Be(AatfEvidenceRedirect.ViewDraftEvidenceRouteName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["aatfId"].Should().Be(AatfId);
            result.RouteValues["evidenceNoteId"].Should().Be(evidenceNoteId);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenSaved_ViewDataShouldHaveNoteStatusAdded()
        {
            //arrange
            var model = ValidModel();
            model.Status = NoteStatus.Returned;
            model.Action = ActionEnum.Save;
            var request = EditRequest();

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be((NoteUpdatedStatusEnum)NoteStatus.Draft);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenReturnedRecordHasBeenSaved_ViewDataShouldHaveNoteStatusAdded()
        {
            //arrange
            var model = ValidModel();
            model.Action = ActionEnum.Save;
            var request = EditRequest(NoteStatus.Returned);

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(NoteUpdatedStatusEnum.ReturnedSaved);
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenSubmitted_ViewDataShouldHaveNoteStatusAdded()
        {
            //arrange
            var model = ValidModel();
            model.Action = ActionEnum.Submit;
            var request = EditRequest(NoteStatus.Submitted);

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(NoteUpdatedStatusEnum.Submitted);
        }

        [Fact]
        public void EditDraftEvidenceNoteGet_ShouldHaveCheckEditEvidenceNoteStatusAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(bool) })
                .Should()
                .BeDecoratedWith<CheckCanEditEvidenceNoteAttribute>();
        }

        [Fact]
        public void EditDraftEvidenceNotePost_ShouldHaveCheckEditEvidenceNoteStatusAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(EditEvidenceNoteViewModel), typeof(Guid), typeof(Guid) })
                .Should()
                .BeDecoratedWith<CheckCanEditEvidenceNoteAttribute>();
        }

        [Fact]
        public async Task EditEvidenceNotePost_GivenCopyPasteAction_Should_RedirectToCopyPaste()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.CopyAndPaste;

            //Act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be(EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName);
            result.RouteValues["organisationId"].Should().Be(OrganisationId);
            result.RouteValues["returnAction"].Should().Be(EvidenceCopyPasteActionConstants.EditEvidenceNoteAction);
        }

        [Fact]
        public async Task EditEvidenceNotePost_NotGivenCopyPasteAction_ShouldNot_RedirectToCopyPaste()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.Submit;

            //Act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //assert
            result.RouteValues["controller"].Should().NotBe(EvidenceCopyPasteActionConstants.EvidenceValueCopyPasteControllerName);
            result.RouteValues["returnAction"].Should().NotBe(EvidenceCopyPasteActionConstants.CreateEvidenceNoteAction);
        }

        [Fact]
        public async Task EditEvidenceNotePost_GivenCopyPasteAction_Should_CallSessionService()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.CopyAndPaste;

            //Act
            var result = await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => SessionService.SetTransferSessionObject(ManageEvidenceController.Session, model, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditEvidenceNotePost_NotGivenCopyPasteAction_ShouldNot_CallSessionService()
        {
            //Arrange
            var model = ValidModel();
            model.Action = ActionEnum.Submit;

            //Act
            await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId);

            //Assert
            A.CallTo(() => SessionService.SetTransferSessionObject(ManageEvidenceController.Session, model, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task EditEvidenceNotePost_GivenApiHasBeenCalledAndThrowsApiExceptionWithInnerInvalidOperationException_ShouldAddErrorToModelState()
        {
            //arrange
            var model = ValidModel();
            var validationMessage = Fixture.Create<string>();
            var exception = new ApiException(Fixture.Create<HttpStatusCode>(), new ApiError()
            {
                ExceptionType = typeof(InvalidOperationException).FullName,
                ExceptionMessage = validationMessage
            });
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Throws(exception);

            //act
            await Record.ExceptionAsync(async () => await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId));

            //assert
            ManageEvidenceController.ModelState.ElementAt(0).Key.Should().BeNullOrEmpty();
            ManageEvidenceController.ModelState.ElementAt(0).Value.Errors.ElementAt(0).ErrorMessage.Should()
                .Be(validationMessage);
        }

        [Fact]
        public async Task EditEvidenceNotePost_GivenApiHasBeenCalledAndThrowsApiExceptionWithWhereInnerItNotInvalidOperationException_ExceptionShouldBeReThrown()
        {
            //arrange
            var model = ValidModel();
            var exception = new ApiException(Fixture.Create<HttpStatusCode>(), new ApiError()
            {
                ExceptionType = Fixture.Create<string>()
            });
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Throws(exception);

            //act
            var result = await Record.ExceptionAsync(async () => await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId));

            //assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task EditEvidenceNoteGet_GivenTrueReturnFromCopyPaste_Should_CallMapperWithExistingModel()
        {
            //Arrange
            var model = ValidModel();
            A.CallTo(() => SessionService.GetTransferSessionObject<EditEvidenceNoteViewModel>(ManageEvidenceController.Session, SessionKeyConstant.EditEvidenceViewModelKey)).Returns(model);

            var schemes = Fixture.CreateMany<OrganisationSchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(noteData);

            //Act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId, true);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<EditNoteMapTransfer>.That.Matches(x => x.AatfId == noteData.AatfData.Id
                && x.ExistingModel == model
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditEvidenceNoteGet_GivenFalseReturnFromCopyPaste_Should_CallMapperWithoutExistingModel()
        {
            //Arrange
            var schemes = Fixture.CreateMany<OrganisationSchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(noteData);

            //Act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId, false);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<EditNoteMapTransfer>.That.Matches(x => x.AatfId == noteData.AatfData.Id
                && x.ExistingModel == null
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes))).MustHaveHappenedOnceExactly();
        }
    }
}