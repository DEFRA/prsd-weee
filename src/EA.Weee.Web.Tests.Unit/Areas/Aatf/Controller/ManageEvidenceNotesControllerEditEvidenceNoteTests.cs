namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
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
        public async Task VEditDraftEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<EvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<EvidenceNoteViewModel>(A<EditNoteMapTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.EditEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

            //asset
            result.Model.Should().Be(model);
        }
    }
}