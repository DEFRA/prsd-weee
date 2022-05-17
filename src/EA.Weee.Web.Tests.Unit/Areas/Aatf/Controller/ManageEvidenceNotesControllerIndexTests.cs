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
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Scheme;
    using Core.Tests.Unit.Helpers;
    using EA.Weee.Requests.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Scheme;
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

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationName = "Organisation";

            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab);

            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenOrganisationId_AatfsShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>.That.Matches(w => w.OrganisationId.Equals(organisationId)))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenOrganisationId_SelectYourAatfViewModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            //assert
            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(
                A<AatfDataToSelectYourAatfViewModelMapTransfer>.That.Matches(
                    a => a.AatfList.Equals(aatfs) && a.FacilityType.Equals(FacilityType.Aatf) &&
                         a.OrganisationId.Equals(organisationId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenRequiredData_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
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
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId) 
                     && m.AatfData.Equals(aatfData) 
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel == null))).MustHaveHappenedOnceExactly();

            foreach (var aatf in selectYourAatfViewModel.AatfList)
            {
                A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                        m => m.Aatfs.Contains(aatf)))).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithEvidenceSummary_GivenRequiredData_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
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
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel == null))).MustHaveHappenedOnceExactly();

            foreach (var aatf in selectYourAatfViewModel.AatfList)
            {
                A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                    m => m.Aatfs.Contains(aatf)))).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        public async void IndexGet_GivenRequiredDataAndFilterModel_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();
            var selectYourAatfViewModel = new SelectYourAatfViewModel() { AatfList = aatfs };
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(
                    A<AatfDataToSelectYourAatfViewModelMapTransfer>._))
                .Returns(selectYourAatfViewModel);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab, filter);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId) 
                     && m.FilterViewModel.Equals(filter.FilterViewModel)))).MustHaveHappenedOnceExactly();

            foreach (var aatf in selectYourAatfViewModel.AatfList)
            {
                A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                    m => m.Aatfs.Contains(aatf)))).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        public async void IndexGetWithDefaultTab_GivenRequiredDataAndFilterModel_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();
            var selectYourAatfViewModel = new SelectYourAatfViewModel() { AatfList = aatfs };
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<SelectYourAatfViewModel>(
                    A<AatfDataToSelectYourAatfViewModelMapTransfer>._))
                .Returns(selectYourAatfViewModel);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab, filter);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel.Equals(filter.FilterViewModel)))).MustHaveHappenedOnceExactly();

            foreach (var aatf in selectYourAatfViewModel.AatfList)
            {
                A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                    m => m.Aatfs.Contains(aatf)))).MustHaveHappenedOnceExactly();
            }
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithDefaultTab_GivenRequiredData_NotesShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.Contains(NoteStatus.Draft) &&
                g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public async Task IndexGetWithDefaultTab_GivenRequiredData_NotesShouldBeRetrieved_EvidenceNotesShouldNotBeRetrievedForInvalidStatus(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Draft) || status.Equals(NoteStatus.Returned))
            {
                return;
            }

            // Arrange
            var organisationName = Faker.Company.Name();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeByOrganisationId>._)).Returns(new SchemeData() { SchemeName = organisationName });

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes);

            //asset
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.Equals(status)))).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_EmptySearchRef_NoteShouldBeRetrieved()
        {
            //arrange
            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            
            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                allowedStatus.SequenceEqual(g.AllowedStatuses) &&
                g.SearchRef == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenSearchFilterParameters_NoteShouldBeRetrieved()
        {
            //arrange
            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes, filter);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.SequenceEqual(allowedStatus) &&
                g.SearchRef.Equals(filter.FilterViewModel.SearchRef)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_NotesShouldNotBeRetrievedWithDraftStatus()
        {
            //arrange
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes, filter);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AllowedStatuses.Contains(NoteStatus.Draft) || g.AllowedStatuses.Contains(NoteStatus.Returned)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenSearchFilterParameters_NoteShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab, filter);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.Contains(NoteStatus.Draft) &&
                g.SearchRef.Equals(filter.FilterViewModel.SearchRef)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredData_NoteViewModelShouldBeBuilt()
        {
            //arrange
            var notes = Fixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(notes);

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes);

            //assert
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId) && e.OrganisationId.Equals(OrganisationId) &&
                         e.Notes.SequenceEqual(notes)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenRequiredData_NoteViewModelShouldBeBuilt(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var notes = Fixture.CreateMany<EvidenceNoteData>().ToList();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(notes);

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab);

            //assert
            A.CallTo(() => Mapper.Map<EditDraftReturnedNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId) && e.OrganisationId.Equals(OrganisationId) &&
                         e.Notes.SequenceEqual(notes)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredData_ModelShouldBeReturned()
        {
            //arrange
            var manageNoteViewModel = new ManageEvidenceNoteViewModel();
            var evidenceNoteViewModel = new AllOtherManageEvidenceNotesViewModel();

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(manageNoteViewModel);
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>._)).Returns(evidenceNoteViewModel);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes) as ViewResult;

            //assert
            var model = result.Model as AllOtherManageEvidenceNotesViewModel;

            model.Should().Be(evidenceNoteViewModel);
            model.ManageEvidenceNoteViewModel.Should().Be(manageNoteViewModel);
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenRequiredData_ModelShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var manageNoteViewModel = new ManageEvidenceNoteViewModel();
            var evidenceNoteViewModel = new EditDraftReturnedNotesViewModel();

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(manageNoteViewModel);
            A.CallTo(() => Mapper.Map<EditDraftReturnedNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>._)).Returns(evidenceNoteViewModel);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab) as ViewResult;

            //assert
            var model = result.Model as EditDraftReturnedNotesViewModel;

            model.Should().Be(evidenceNoteViewModel);
            model.ManageEvidenceNoteViewModel.Should().Be(manageNoteViewModel);
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenAction_EditDraftReturnedNotesOverviewViewShouldBeReturned()
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes) as ViewResult;

            result.ViewName.Should().Be("Overview/ViewAllOtherEvidenceOverview");
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenAction_EditDraftReturnedNotesOverviewViewShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab) as ViewResult;

            result.ViewName.Should().Be("Overview/EditDraftReturnedNotesOverview");
        }

        [Fact]
        public void IndexGet_ShouldBeDecoratedWith_HttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(Guid), typeof(Guid), typeof(ManageEvidenceOverviewDisplayOption?), typeof(ManageEvidenceNoteViewModel) }, null)
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

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAction_EvidenceSummaryOverviewViewShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab) as ViewResult;

            result.ViewName.Should().Be("Overview/EvidenceSummaryOverview");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAatf_EvidenceSummaryShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            A.CallTo(() =>
                    WeeeClient.SendAsync(A<string>._,
                        A<GetAatfSummaryRequest>.That.Matches(g => g.AatfId.Equals(aatfId))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAatfSummary_ModelShouldBeBuilt(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var summary = Fixture.Create<AatfEvidenceSummaryData>();

            A.CallTo(() =>
                WeeeClient.SendAsync(A<string>._,
                    A<GetAatfSummaryRequest>._)).Returns(summary);

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab);

            A.CallTo(() => Mapper.Map<ManageEvidenceSummaryViewModel>(A<EvidenceSummaryMapTransfer>.That.Matches(e =>
                e.AatfEvidenceSummaryData.Equals(summary) && e.AatfId.Equals(aatfId) &&
                e.OrganisationId.Equals(organisationId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_Model_ModelShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var model = Fixture.Create<ManageEvidenceSummaryViewModel>();
            var evidenceNoteViewModel = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<ManageEvidenceSummaryViewModel>(A<EvidenceSummaryMapTransfer>._)).Returns(model);
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._))
                .Returns(evidenceNoteViewModel);

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab) as ViewResult;

            var resultModel = (ManageEvidenceSummaryViewModel)result.Model;
            resultModel.Should().Be(model);
            resultModel.ManageEvidenceNoteViewModel.Should().Be(evidenceNoteViewModel);
        }
    }
}