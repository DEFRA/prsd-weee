namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Constant;
    using Core.AatfReturn;
    using EA.Weee.Requests.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Weee.Requests.AatfReturn;
    using Xunit;

    public class ManageEvidenceNotesControllerIndexTests : ManageEvidenceNotesControllerTestsBase
    {
        public ManageEvidenceNotesControllerIndexTests()
        {
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._))
                .Returns(new ManageEvidenceNoteViewModel());

            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(A<AatfDataToSelectYourAatfViewModelMapTransfer>._))
                .Returns(new SelectYourAatfViewModel() { AatfList = new List<AatfData>() });
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationName = "Organisation";

            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await ManageEvidenceController.Index(OrganisationId, AatfId);

            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_AatfsShouldBeRetrieved()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId.Equals(organisationId)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void IndexGet_GivenOrganisationId_SelectYourAatfViewModelMapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId);

            //assert
            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(
                A<AatfDataToSelectYourAatfViewModelMapTransfer>.That.Matches(
                    a => a.AatfList.Equals(aatfs) && a.FacilityType.Equals(FacilityType.Aatf) &&
                         a.OrganisationId.Equals(organisationId)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenRequiredData_ModelMapperShouldBeCalled()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();
            var selectYourAatfViewModel = new SelectYourAatfViewModel() { AatfList = aatfs };

            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(
                    A<AatfDataToSelectYourAatfViewModelMapTransfer>._))
                .Returns(selectYourAatfViewModel);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId) 
                     && m.AatfData.Equals(aatfData) 
                     && m.OrganisationId.Equals(organisationId)))).MustHaveHappenedOnceExactly();

            foreach (var aatf in selectYourAatfViewModel.AatfList)
            {
                A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                        m => m.Aatfs.Contains(aatf)))).MustHaveHappenedOnceExactly();
            }
        }

        [Fact]
        public async void IndexGet_GivenRequiredData_ModelShouldBeReturned()
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var model = new ManageEvidenceNoteViewModel();
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(model);

            //act
            var result = await ManageEvidenceController.Index(organisationId, aatfId) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public void IndexGet_ShouldBeDecoratedWith_HttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(Guid), typeof(Guid) }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void IndexPost_ShouldBeDecoratedWith_Attributes()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(ManageEvidenceNoteViewModel) }, null)
            .Should()
            .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void IndexPost_ValidViewModel_PageRedirectsCreateEvidenceNote()
        {
            var model = new ManageEvidenceNoteViewModel()
            {
                OrganisationId = Fixture.Create<Guid>(),
                AatfId = Fixture.Create<Guid>(),
            };

            var result = ManageEvidenceController.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("CreateEvidenceNote");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
            result.RouteValues["organisationId"].Should().Be(model.OrganisationId);
            result.RouteValues["aatfId"].Should().Be(model.AatfId); 
        }
    }
}