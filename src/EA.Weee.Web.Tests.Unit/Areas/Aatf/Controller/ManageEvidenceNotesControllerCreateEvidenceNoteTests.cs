﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Web.ApiClient;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Areas.AatfEvidence.Controllers;
    using Web.Filters;
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
        public void CreateEvidenceNoteGet_ShouldHaveCheckCanCreateEvidenceNoteAttributeAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(bool) })
                .Should()
                .BeDecoratedWith<CheckCanCreateEvidenceNoteAttribute>();
        }

        [Fact]
        public void CreateEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(bool) })
                .Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void CreateEvidenceNoteGet_ShouldHaveNoCacheAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int), typeof(bool) })
                .Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, Fixture.Create<int>()) as ViewResult;

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
            await ManageEvidenceController.CreateEvidenceNote(organisationId, AatfId, Fixture.Create<int>());

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_OrganisationSchemesListShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, Fixture.Create<int>());

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS.Equals(true)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var complianceYear = Fixture.Create<int>();
            var schemes = Fixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);

            //act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, complianceYear);

            //assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(
                A<CreateNoteMapTransfer>.That.Matches(c => 
                    c.Schemes.Equals(schemes) && 
                    c.ExistingModel == null && 
                    c.AatfId.Equals(AatfId) &&
                    c.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenViewModel_ViewModelShouldBeReturned()
        {
            //arrange
            var model = new EditEvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, Fixture.Create<int>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public void CreateEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("CreateEvidenceNote", new[] { typeof(EditEvidenceNoteViewModel), typeof(Guid), typeof(Guid) })
                .Should()
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
        public async Task CreateEvidenceNotePost_GivenInvalidModel_OrganisationSchemesListShouldBeRetrieved()
        {
            //arrange
            AddModelError();

            //act
            await ManageEvidenceController.CreateEvidenceNote(A.Dummy<EditEvidenceNoteViewModel>(), OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetOrganisationScheme>.That.Matches(r => r.IncludePBS.Equals(true)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenInvalidModel_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = Fixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);
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
                RecipientId = Guid.NewGuid()
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

        [Fact]
        public async Task CreateEvidenceNotePost_GivenApiHasBeenCalledAndThrowsApiExceptionWithInnerInvalidOperationException_ShouldAddErrorToModelState()
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
            await Record.ExceptionAsync(async () => await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId));

            //assert
            ManageEvidenceController.ModelState.ElementAt(0).Key.Should().BeNullOrEmpty();
            ManageEvidenceController.ModelState.ElementAt(0).Value.Errors.ElementAt(0).ErrorMessage.Should()
                .Be(validationMessage);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenApiHasBeenCalledAndThrowsApiExceptionWithWhereInnerItNotInvalidOperationException_ExceptionShouldBeReThrown()
        {
            //arrange
            var model = ValidModel();
            var exception = new ApiException(Fixture.Create<HttpStatusCode>(), new ApiError()
            {
                ExceptionType = Fixture.Create<string>()
            });
            A.CallTo(() => WeeeClient.SendAsync<Guid>(A<string>._, A<EvidenceNoteBaseRequest>._)).Throws(exception);

            //act
            var result = await Record.ExceptionAsync(async () => await ManageEvidenceController.CreateEvidenceNote(model, OrganisationId, AatfId));

            //assert
            result.Should().NotBeNull();
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
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be((NoteUpdatedStatusEnum)status);
        }

        [Fact]
        public async Task CreateEvidenceNotePost_GivenModelStateErrors_ErrorsShouldBeOrdered()
        {
            //arrange
            var model = ValidModel();
            var request = Request(NoteStatus.Submitted);

            A.CallTo(() => CreateRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            ManageEvidenceController.ModelState.AddModelError("ProtocolValue", new Exception());
            ManageEvidenceController.ModelState.AddModelError("RecipientId", new Exception());
            ManageEvidenceController.ModelState.AddModelError("CategoryValues2", new Exception());
            ManageEvidenceController.ModelState.AddModelError("WasteTypeValue", new Exception());
            ManageEvidenceController.ModelState.AddModelError("StartDate", new Exception());
            ManageEvidenceController.ModelState.AddModelError("EndDate", new Exception());
            ManageEvidenceController.ModelState.AddModelError("Recipient-auto", new Exception());
            ManageEvidenceController.ModelState.AddModelError("CategoryValues", new Exception());

            //act
            await ManageEvidenceController.CreateEvidenceNote(model, Fixture.Create<Guid>(), Fixture.Create<Guid>());

            //assert
            ManageEvidenceController.ModelState.ElementAt(0).Key.Should().Be("StartDate");
            ManageEvidenceController.ModelState.ElementAt(1).Key.Should().Be("EndDate");
            ManageEvidenceController.ModelState.ElementAt(2).Key.Should().Be("RecipientId");
            ManageEvidenceController.ModelState.ElementAt(3).Key.Should().Be("Recipient-auto");
            ManageEvidenceController.ModelState.ElementAt(4).Key.Should().Be("WasteTypeValue");
            ManageEvidenceController.ModelState.ElementAt(5).Key.Should().Be("ProtocolValue");
            ManageEvidenceController.ModelState.ElementAt(6).Key.Should().Be("CategoryValues2");
            ManageEvidenceController.ModelState.ElementAt(7).Key.Should().Be("CategoryValues");
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_Should_CallAndClearSessionService()
        {
            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, Fixture.Create<int>(), true);

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

            var schemes = Fixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);

            var complianceYear = Fixture.Create<int>();

            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, complianceYear, true);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>.That.Matches(x => x.AatfId == AatfId
                && x.ExistingModel == model
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes 
                && x.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateEvidenceNoteGet_GivenFalseReturnFromCopyPaste_Should_CallMapperWithoutExistingModel()
        {
            //Arrange
            var schemes = Fixture.CreateMany<EntityIdDisplayNameData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetOrganisationScheme>._)).Returns(schemes);

            var complianceYear = Fixture.Create<int>();

            //Act
            await ManageEvidenceController.CreateEvidenceNote(OrganisationId, AatfId, complianceYear, false);

            //Assert
            A.CallTo(() => Mapper.Map<EditEvidenceNoteViewModel>(A<CreateNoteMapTransfer>.That.Matches(x => x.AatfId == AatfId
                && x.ExistingModel == null
                && x.OrganisationId == OrganisationId
                && x.Schemes == schemes 
                && x.ComplianceYear == complianceYear))).MustHaveHappenedOnceExactly();
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