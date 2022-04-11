﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
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
    using Web.Infrastructure;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.Scheme;
    using Xunit;

    public class ManageEvidenceNotesControllerEditEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        private readonly EvidenceNoteData noteData;

        public ManageEvidenceNotesControllerEditEvidenceNoteTests()
        {
            noteData = Fixture.Create<EvidenceNoteData>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteRequest>._)).Returns(noteData);
        }

        [Fact]
        public void EditDraftEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_SchemesListShouldBeRetrieved()
        {
            //act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_GivenRequestData_EditEvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            var schemes = Fixture.CreateMany<SchemeData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteRequest>._)).Returns(noteData);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);

            //act
            await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<EditNoteMapTransfer>.That.Matches(
                v => v.Schemes.Equals(schemes) &&
                     v.NoteData.Equals(noteData) &&
                     v.OrganisationId.Equals(OrganisationId) &&
                     v.AatfId.Equals(noteData.AatfData.Id) && 
                     v.ExistingModel == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<EvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public void EditDraftEvidenceNotePost_ShouldHaveHttpPostAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("EditEvidenceNote", new[] { typeof(EvidenceNoteViewModel), typeof(Guid), typeof(Guid) }).Should()
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
            await ManageEvidenceController.EditEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), organisationId, AatfId);

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
            await ManageEvidenceController.EditEvidenceNote(A.Dummy<EvidenceNoteViewModel>(), OrganisationId, AatfId);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._,
                A<GetSchemesExternal>.That.Matches(r => r.IncludeWithdrawn.Equals(false)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_ViewModelMapperShouldBeCalled()
        {
            //arrange
            var schemes = Fixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemesExternal>._)).Returns(schemes);
            var model = A.Dummy<EvidenceNoteViewModel>();
            AddModelError();

            //act
            await ManageEvidenceController.EditEvidenceNote(model, OrganisationId, AatfId);

            //assert
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(
                A<EditNoteMapTransfer>.That.Matches(c => c.Schemes.Equals(schemes)
                                                         && c.ExistingModel.Equals(model)
                                                         && c.OrganisationId.Equals(OrganisationId)
                                                         && c.AatfId.Equals(AatfId) &&
                                                         c.NoteData == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditDraftEvidenceNotePost_GivenInvalidModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new EvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).Returns(model);
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
            var model = new EvidenceNoteViewModel()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ReceivedId = Guid.NewGuid()
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

        [Theory]
        [InlineData(NoteStatus.Draft)]
        [InlineData(NoteStatus.Submitted)]
        public async Task EditDraftEvidenceNotePost_GivenApiHasBeenCalled_ViewDataShouldHaveNoteStatusAdded(NoteStatus status)
        {
            //arrange
            var model = ValidModel();
            var request = EditRequest(status);

            A.CallTo(() => EditRequestCreator.ViewModelToRequest(A<EvidenceNoteViewModel>._)).Returns(request);

            //act
            await ManageEvidenceController.EditEvidenceNote(model, A.Dummy<Guid>(), A.Dummy<Guid>());

            //assert
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus].Should().Be(status);
        }
    }
}