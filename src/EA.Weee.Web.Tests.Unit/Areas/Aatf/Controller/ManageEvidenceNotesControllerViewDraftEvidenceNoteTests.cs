namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfEvidence;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.ViewModels;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class ManageEvidenceNotesControllerViewDraftEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ViewDraftEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewDraftEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, AatfId, EvidenceNoteId) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await ManageEvidenceController.ViewDraftEvidenceNote(organisationId, AatfId, EvidenceNoteId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenDraftNoteHasBeenCreated_ModelSuccessMessageShouldBeSet()
        {
            //arrange
            var evidenceNoteData = Fixture.Create<EvidenceNoteData>();

            ManageEvidenceController.ViewData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Draft;
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteRequest>._)).Returns(evidenceNoteData);

            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, AatfId, EvidenceNoteId) as ViewResult;

            //assert
            var model = result.Model as ViewEvidenceNoteViewModel;
            model.SuccessMessage.Should()
                .Be($"You have successfully saved the evidence note with reference ID E{evidenceNoteData.Reference} as a draft");
            model.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenSubmittedNoteHasBeenCreated_ModelSuccessMessageShouldBeSet()
        {
            //arrange
            var evidenceNoteData = Fixture.Create<EvidenceNoteData>();

            ManageEvidenceController.ViewData[ViewDataConstant.EvidenceNoteStatus] = NoteStatus.Submitted;
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteRequest>._)).Returns(evidenceNoteData);

            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, AatfId, EvidenceNoteId) as ViewResult;

            //assert
            var model = result.Model as ViewEvidenceNoteViewModel;
            model.SuccessMessage.Should()
                .Be($"You have successfully submitted the evidence note with reference ID E{evidenceNoteData.Reference}");
            model.DisplayMessage.Should().BeTrue();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenRecordWasNotCreated_SuccessMessageShouldNotBeShown()
        {
            //arrange

            ManageEvidenceController.ViewData[ViewDataConstant.EvidenceNoteStatus] = null;

            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, AatfId, EvidenceNoteId) as ViewResult;

            //assert
            var model = result.Model as ViewEvidenceNoteViewModel;
            model.DisplayMessage.Should().BeFalse();
        }
    }
}