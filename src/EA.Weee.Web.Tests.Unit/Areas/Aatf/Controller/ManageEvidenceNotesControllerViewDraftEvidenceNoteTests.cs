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
    using Web.Filters;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ManageEvidenceNotesControllerViewDraftEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ViewDraftEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewDraftEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ViewDraftEvidenceNoteGet_ShouldHaveNoCacheAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewDraftEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(int) }).Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId) as ViewResult;

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
            await ManageEvidenceController.ViewDraftEvidenceNote(organisationId, EvidenceNoteId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenEvidenceId_EvidenceNoteShouldBeRetrieved()
        {
            //arrange

            //act
            await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>.That.Matches(
                g => g.EvidenceNoteId.Equals(EvidenceNoteId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenRequestData_EvidenceNoteModelShouldBeBuilt()
        {
            //arrange
            var data = Fixture.Create<EvidenceNoteData>();
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = null;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(data);

            //act
            await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(data) &&
                     v.NoteStatus == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task ViewDraftEvidenceNoteGet_GivenRequestDataAndSuccessTempData_EvidenceNoteModelShouldBeBuilt(NoteStatus status)
        {
            //arrange
            var data = Fixture.Create<EvidenceNoteData>();
            ManageEvidenceController.TempData[ViewDataConstant.EvidenceNoteStatus] = status;

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetEvidenceNoteForAatfRequest>._)).Returns(data);

            //act
            await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId);

            //asset
            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>.That.Matches(
                v => v.EvidenceNoteData.Equals(data) &&
                     v.NoteStatus.Equals(status)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = Fixture.Create<ViewEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<ViewEvidenceNoteViewModel>(A<ViewEvidenceNoteMapTransfer>._)).Returns(model);

            //act
            var result =
                await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId) as
                    ViewResult;

            //asset
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_GivenPageNumber_ShouldPopulateViewBagPage()
        {
            // Arrange
            var pageNumber = 3;

            //act
            var result = await ManageEvidenceController.ViewDraftEvidenceNote(OrganisationId, EvidenceNoteId, pageNumber) as ViewResult;

            //assert
            Assert.Equal(pageNumber, result.ViewBag.Page);
        }
    }
}