﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
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
    using Core.Helpers;
    using Core.Scheme;
    using EA.Prsd.Core;
    using EA.Weee.Requests.Aatf;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.Filters;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Scheme;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ManageEvidenceNotesControllerIndexTests : ManageEvidenceNotesControllerTestsBase
    {
        private readonly DateTime currentDate;

        public ManageEvidenceNotesControllerIndexTests()
        {
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByOrganisation>._)).Returns(Fixture.CreateMany<AatfData>().ToList());

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(new ManageEvidenceNoteViewModel());

            currentDate = new DateTime(2019, 1, 1);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationName = "Organisation";

            A.CallTo(() => Cache.FetchOrganisationName(A<Guid>._)).Returns(organisationName);

            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab?.ToDisplayString(), null, 1);

            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenNullManageEvidenceNotesViewModelAndSessionSelectedYearIsNull_ModelMapperShouldBeCalledWithCorrectYear(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            //arrange
            var currentDate = Fixture.Create<DateTime>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab?.ToDisplayString(), null, 1);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m => 
                m.ComplianceYear == currentDate.Year))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenNullManageEvidenceNotesViewModelAndSessionSelectedYearHasValue_ModelMapperShouldBeCalledWithCorrectYear(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            //arrange
            var currentDate = Fixture.Create<DateTime>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null, null, 1);

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(m => 
                m.ComplianceYear == currentDate.Year))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenMappedEvidenceNotesViewModel_MappedManageEvidenceNotesModelShouldBeReturned(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            //arrange
            var currentDate = Fixture.Create<DateTime>();
            var model = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(model);
            
            //act
            var result = await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null) as ViewResult;

            var convertedModel = result.Model as ManageEvidenceNoteOverviewViewModel;

            convertedModel.ManageEvidenceNoteViewModel.Should().Be(model);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenOrganisationId_ApiShouldBeCalled(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null, null, 1);

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, 
                A<GetAatfByIdExternal>.That.Matches(w => w.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenOrganisationId_AatfsShouldBeRetrieved(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(SystemTime.UtcNow);

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null, null, 1);

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(organisationId)).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGet_GivenRequiredData_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null, null, 1);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId) 
                     && m.AatfData.Equals(aatfData) 
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel == null 
                     && m.CurrentDate == currentDate &&
                     m.Aatfs.SequenceEqual(aatfs)))).MustHaveHappenedOnceExactly();
        }
        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithEvidenceSummary_GivenRequiredData_ModelMapperShouldBeCalled(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            //arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.HasValue ? EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab.Value) : null);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel == null 
                     && m.CurrentDate == currentDate
                     && m.Aatfs.SequenceEqual(aatfs)))).MustHaveHappenedOnceExactly();
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
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, EA.Weee.Web.Extensions.DisplayExtensions.ToDisplayString(selectedTab), filter, 1);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId) 
                     && m.FilterViewModel.Equals(filter.FilterViewModel) 
                     && m.CurrentDate == currentDate
                     && m.Aatfs.SequenceEqual(aatfs) 
                     && m.ComplianceYear == filter.SelectedComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenRequiredDataAndRecipientWasteStatusFilterViewModel_ModelMapperShouldBeCalled()
        {
            //arrange
            var selectedTab = ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes;
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();
            
            var recipientFilter = new RecipientWasteStatusFilterViewModel();
            var viewModel = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.RecipientWasteStatusFilterViewModel, recipientFilter).Create();

            A.CallTo(() => Mapper.Map<RecipientWasteStatusFilterViewModel>(
                    A<RecipientWasteStatusFilterBase>._))
                .Returns(recipientFilter);

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.ToDisplayString(), viewModel, 1);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.RecipientWasteStatusFilterViewModel.Equals(recipientFilter)
                     && m.CurrentDate == currentDate
                     && m.Aatfs.SequenceEqual(aatfs)
                     && m.ComplianceYear == viewModel.SelectedComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGet_GivenRequiredDataAndSubmittedDatesFilterViewModel_ModelMapperShouldBeCalled()
        {
            //arrange
            var selectedTab = ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes;
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var aatfs = Fixture.CreateMany<AatfData>().ToList();
            var aatfData = Fixture.Create<AatfData>();

            var submittedDateFilter = new SubmittedDatesFilterViewModel();
            var viewModel = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SubmittedDatesFilterViewModel, submittedDateFilter).Create();

            A.CallTo(() => Mapper.Map<SubmittedDatesFilterViewModel>(
                    A<SubmittedDateFilterBase>._))
                .Returns(submittedDateFilter);

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.ToDisplayString(), viewModel, 1);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.SubmittedDatesFilterViewModel.Equals(submittedDateFilter)
                     && m.CurrentDate == currentDate
                     && m.Aatfs.SequenceEqual(aatfs) 
                     && m.ComplianceYear == viewModel.SelectedComplianceYear))).MustHaveHappenedOnceExactly();
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
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Cache.FetchAatfDataForOrganisationData(A<Guid>._)).Returns(aatfs);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfByIdExternal>._)).Returns(aatfData);

            //act
            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.ToDisplayString(), filter, 1);

            //assert
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>.That.Matches(
                m => m.AatfId.Equals(aatfId)
                     && m.AatfData.Equals(aatfData)
                     && m.OrganisationId.Equals(organisationId)
                     && m.FilterViewModel.Equals(filter.FilterViewModel)
                     && m.CurrentDate == currentDate
                     && m.Aatfs.SequenceEqual(aatfs) 
                     && m.ComplianceYear == filter.SelectedComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithDefaultTab_GivenRequiredData_NotesShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            int complianceYear = 2018;
            ManageEvidenceNoteViewModel vm = new ManageEvidenceNoteViewModel { SelectedComplianceYear = complianceYear };
            var pageNumber = Fixture.Create<int>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString(), vm, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.ComplianceYear.Equals(complianceYear) && 
                g.AllowedStatuses.Contains(NoteStatus.Draft) &&
                g.SearchRef == null &&
                g.PageSize == 10 &&
                g.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithDefaultTab_GivenRequiredDataAndPageNumber_NotesShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            int complianceYear = 2018;
            ManageEvidenceNoteViewModel vm = new ManageEvidenceNoteViewModel { SelectedComplianceYear = complianceYear };
            const int pageNumber = 11;

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString(), vm, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.PageSize == 10 &&
                g.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
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
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes.ToDisplayString());

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
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), null, 3);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                allowedStatus.SequenceEqual(g.AllowedStatuses) &&
                g.SearchRef == null &&
                g.PageSize.Equals(10) &&
                g.PageNumber.Equals(3)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenSearchFilterParameters_NoteShouldBeRetrieved()
        {
            //arrange
            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, 
                ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), filter, 3);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.SequenceEqual(allowedStatus) &&
                g.SearchRef.Equals(filter.FilterViewModel.SearchRef) &&
                g.PageSize.Equals(10) &&
                g.PageNumber.Equals(3)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRecipientWasteStatusFilterParameters_NoteShouldBeRetrieved()
        {
            //arrange
            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            var received = Fixture.Create<Guid>();
            var wasteType = Fixture.Create<WasteType?>();
            var statusFilter = Fixture.Create<NoteStatus?>();
            var recipientWasteStatusFilter = Fixture.Build<RecipientWasteStatusFilterViewModel>()
                .With(r => r.ReceivedId, received)
                .With(r => r.WasteTypeValue, wasteType)
                .With(r => r.NoteStatusValue, statusFilter)
                .Create();
            var viewModel = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.RecipientWasteStatusFilterViewModel, recipientWasteStatusFilter).Create();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), viewModel, 3);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.SequenceEqual(allowedStatus) &&
                g.RecipientId.Value.Equals(recipientWasteStatusFilter.ReceivedId.Value) &&
                g.WasteTypeId.Value.Equals(recipientWasteStatusFilter.WasteTypeValue.Value) &&
                g.NoteStatusFilter.Equals(recipientWasteStatusFilter.NoteStatusValue) && 
                g.PageSize.Equals(10) &&
                g.PageNumber.Equals(3)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenSubmittedDatesFilterParameters_NoteShouldBeRetrieved()
        {
            //arrange
            var allowedStatus = new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected };
            var startDateFilter = Fixture.Create<DateTime>();
            var endDateFilter = Fixture.Create<DateTime>();
            var submittedDatesFilter = Fixture.Build<SubmittedDatesFilterViewModel>()
                .With(s => s.StartDate, startDateFilter)
                .With(s => s.EndDate, endDateFilter)
                .Create();
            var viewModel = Fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SubmittedDatesFilterViewModel, submittedDatesFilter).Create();

            const int pageNumber = 11;

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), viewModel, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.SequenceEqual(allowedStatus) &&
                g.StartDateSubmitted.Value.Equals(submittedDatesFilter.StartDate) &&
                g.EndDateSubmitted.Value.Equals(submittedDatesFilter.EndDate) &&
                g.PageSize == 10 &&
                g.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_NotesShouldNotBeRetrievedWithDraftStatus()
        {
            //arrange
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), filter, 1);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AllowedStatuses.Contains(NoteStatus.Draft) || g.AllowedStatuses.Contains(NoteStatus.Returned)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenSearchFilterParameters_NoteShouldBeRetrieved(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var filter = Fixture.Create<ManageEvidenceNoteViewModel>();
            const int pageNumber = 11;

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString(), filter, pageNumber);

            //assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(g =>
                g.AatfId.Equals(AatfId) &&
                g.OrganisationId.Equals(OrganisationId) &&
                g.AllowedStatuses.Contains(NoteStatus.Draft) &&
                g.SearchRef.Equals(filter.FilterViewModel.SearchRef) &&
                g.PageSize == 10 &&
                g.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredData_NoteViewModelShouldBeBuilt()
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2019, 1, 1);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(date);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(noteData);
            var pageNumber = 5;

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), null, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId) 
                         && e.OrganisationId.Equals(OrganisationId) &&
                         e.NoteData.Equals(noteData) &&
                         e.CurrentDate == date &&
                         e.PageNumber.Equals(pageNumber) &&
                         e.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredDataAndExistingModel_NoteViewModelShouldBeBuilt()
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2019, 1, 1);
            var existingModel = Fixture.Create<ManageEvidenceNoteViewModel>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(date);
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(noteData);
            var pageNumber = 5;

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), existingModel, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId)
                         && e.OrganisationId.Equals(OrganisationId) &&
                         e.NoteData.Equals(noteData) &&
                         e.CurrentDate == date &&
                         e.ManageEvidenceNoteViewModel == existingModel &&
                         e.PageNumber.Equals(pageNumber) &&
                         e.PageSize == 10))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenRequiredData_NoteViewModelShouldBeBuilt(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteSearchDataResult>();
            var pageNumber = Fixture.Create<int>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(noteData);
            
            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString(), null, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<EditDraftReturnedNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId) && e.OrganisationId.Equals(OrganisationId) &&
                         e.NoteData.Equals(noteData) &&
                         e.PageSize == 10 &&
                         e.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenRequiredDataAndExistingModel_NoteViewModelShouldBeBuilt(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var noteData = Fixture.Create<EvidenceNoteSearchDataResult>();
            var existingModel = Fixture.Create<ManageEvidenceNoteViewModel>();
            var pageNumber = Fixture.Create<int>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(noteData);

            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString(), existingModel, pageNumber);

            //assert
            A.CallTo(() => Mapper.Map<EditDraftReturnedNotesViewModel>(
                A<EvidenceNotesViewModelTransfer>.That.Matches(
                    e => e.AatfId.Equals(AatfId) && 
                         e.OrganisationId.Equals(OrganisationId) &&
                         e.NoteData.Equals(noteData) &&
                         e.ManageEvidenceNoteViewModel == existingModel &&
                         e.PageSize == 10 &&
                         e.PageNumber == pageNumber))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredData_ModelShouldBeReturned()
        {
            //arrange
            var manageNoteViewModel = new ManageEvidenceNoteViewModel();
            var evidenceNoteViewModel = new AllOtherManageEvidenceNotesViewModel();

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(manageNoteViewModel);
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(A<EvidenceNotesViewModelTransfer>._)).Returns(evidenceNoteViewModel);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), null, 1) as ViewResult;

            //assert
            var model = result.Model as AllOtherManageEvidenceNotesViewModel;

            model.Should().Be(evidenceNoteViewModel);
            model.ManageEvidenceNoteViewModel.Should().Be(manageNoteViewModel);
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenRequiredData_SubmittedDatesMapperIsCalled()
        {
            //arrange
            var startDate = Fixture.Create<DateTime>();
            var endDate = Fixture.Create<DateTime>();
            var submittedFilter = Fixture.Build<SubmittedDatesFilterViewModel>()
                .With(s => s.StartDate, startDate)
                .With(s => s.EndDate, endDate)
                .Create();
            var manageNoteViewModel = Fixture.Build<ManageEvidenceNoteViewModel>().With(r => r.SubmittedDatesFilterViewModel, submittedFilter).Create();
            var allOtherNotes = Fixture.Build<AllOtherManageEvidenceNotesViewModel>().With(a => a.ManageEvidenceNoteViewModel, manageNoteViewModel).Create();
            var noteData = Fixture.Create<EvidenceNoteSearchDataResult>();

            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>._)).Returns(noteData);
            A.CallTo(() => Mapper.Map<AllOtherManageEvidenceNotesViewModel>(A<EvidenceNotesViewModelTransfer>._))
                .Returns(allOtherNotes);
        
            //act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), manageNoteViewModel, 1);

            //assert
            A.CallTo(() => Mapper.Map<SubmittedDatesFilterViewModel>(
                A<SubmittedDateFilterBase>.That.Matches(e => e.StartDate.Equals(startDate) &&
                e.EndDate.Equals(endDate)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenNullManageEvidenceNoteViewModel_OrganisationSchemeDataShouldBeRetrieved()
        {
            // arrange
            var currentDate = Fixture.Create<DateTime>();
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetApiUtcDate>._)).Returns(currentDate);

            // act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), null, 1);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeDataForFilterRequest>.That.Matches(
                g => g.ComplianceYear == currentDate.Year &&
                     g.AatfId == AatfId &&
                     g.RecipientOrTransfer == RecipientOrTransfer.Recipient &&
                     g.AllowedStatuses.SequenceEqual(new List<NoteStatus>
                     {
                         NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected
                     }) &&
                     g.AllowedNoteTypes.SequenceEqual(new List<NoteType>() { NoteType.Evidence }))))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void IndexGetWithViewAllOtherEvidenceNotesTabSelected_GivenManageEvidenceNoteViewModel_OrganisationSchemeDataShouldBeRetrieved()
        {
            // arrange
            var existingManageEvidenceNoteViewModel = Fixture.Create<ManageEvidenceNoteViewModel>();

            // act
            await ManageEvidenceController.Index(OrganisationId, AatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), existingManageEvidenceNoteViewModel, 1);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetSchemeDataForFilterRequest>.That.Matches(
                g => g.ComplianceYear == existingManageEvidenceNoteViewModel.SelectedComplianceYear &&
                     g.AatfId == AatfId &&
                     g.RecipientOrTransfer == RecipientOrTransfer.Recipient &&
                     g.AllowedStatuses.SequenceEqual(new List<NoteStatus>
                     {
                         NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Void, NoteStatus.Rejected
                     }) &&
                     g.AllowedNoteTypes.SequenceEqual(new List<NoteType>() { NoteType.Evidence }))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenRequiredData_ModelShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            //arrange
            var manageNoteViewModel = new ManageEvidenceNoteViewModel();
            var evidenceNoteViewModel = new EditDraftReturnedNotesViewModel();

            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(manageNoteViewModel);
            A.CallTo(() => Mapper.Map<EditDraftReturnedNotesViewModel>(A<EvidenceNotesViewModelTransfer>._)).Returns(evidenceNoteViewModel);

            //act
            var result = await ManageEvidenceController.Index(OrganisationId, AatfId, selectedTab.ToDisplayString()) as ViewResult;

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

            var result = await ManageEvidenceController.Index(organisationId, aatfId, ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString(), null, 1) as ViewResult;

            result.ViewName.Should().Be("Overview/ViewAllOtherEvidenceOverview");
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        public async void IndexGetWithEditDraftAndReturnedNotesTab_GivenAction_EditDraftReturnedNotesOverviewViewShouldBeReturned(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.ToDisplayString()) as ViewResult;

            result.ViewName.Should().Be("Overview/EditDraftReturnedNotesOverview");
        }

        [Fact]
        public void IndexGet_ShouldBeDecoratedWith_HttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(Guid), typeof(Guid), typeof(string), typeof(ManageEvidenceNoteViewModel), typeof(int) }, null)
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
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAction_EvidenceSummaryOverviewViewShouldBeReturned(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab?.ToDisplayString()) as ViewResult;

            result.ViewName.Should().Be("Overview/EvidenceSummaryOverview");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAatf_EvidenceSummaryShouldBeRetrieved(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var complianceYear = 2000;
            ManageEvidenceNoteViewModel vm = new ManageEvidenceNoteViewModel { SelectedComplianceYear = complianceYear };

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab?.ToDisplayString(), vm);

            A.CallTo(() =>
                    WeeeClient.SendAsync(A<string>._,
                        A<GetAatfSummaryRequest>.That.Matches(g => 
                            g.AatfId.Equals(aatfId) && 
                            g.ComplianceYear.Equals(complianceYear))))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_GivenAatfSummary_ModelShouldBeBuilt(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            AatfEvidenceSummaryData summary = Fixture.Create<AatfEvidenceSummaryData>();

            A.CallTo(() =>
                WeeeClient.SendAsync(A<string>._,
                    A<GetAatfSummaryRequest>._)).Returns(summary);

            await ManageEvidenceController.Index(organisationId, aatfId, selectedTab?.ToDisplayString());

            A.CallTo(() => Mapper.Map<ManageEvidenceSummaryViewModel>(A<EvidenceSummaryMapTransfer>.That.Matches(e =>
                e.AatfEvidenceSummaryData.Equals(summary) && 
                e.AatfId.Equals(aatfId) &&
                e.OrganisationId.Equals(organisationId)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(ManageEvidenceOverviewDisplayOption.EvidenceSummary)]
        public async void IndexGetWithDefaultAndEvidenceSummaryTab_Model_ModelShouldBeReturned(ManageEvidenceOverviewDisplayOption? selectedTab)
        {
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var model = Fixture.Create<ManageEvidenceSummaryViewModel>();
            var evidenceNoteViewModel = Fixture.Create<ManageEvidenceNoteViewModel>();

            A.CallTo(() => Mapper.Map<ManageEvidenceSummaryViewModel>(A<EvidenceSummaryMapTransfer>._)).Returns(model);
            A.CallTo(() => Mapper.Map<ManageEvidenceNoteViewModel>(A<ManageEvidenceNoteTransfer>._)).Returns(evidenceNoteViewModel);

            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab?.ToDisplayString()) as ViewResult;

            var resultModel = (ManageEvidenceSummaryViewModel)result.Model;
            resultModel.Should().Be(model);
            resultModel.ManageEvidenceNoteViewModel.Should().Be(evidenceNoteViewModel);
        }

        [Fact]
        public void IndexGet_ShouldBeDecoratedWith_NoCacheAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("Index", new[] { typeof(Guid), typeof(Guid), typeof(string), typeof(ManageEvidenceNoteViewModel), typeof(int) })
                .Should()
                .BeDecoratedWith<NoCacheFilterAttribute>();
        }

        [Theory]
        [InlineData(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes)]
        [InlineData(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes)]
        public async void IndexGet_ModelPopulatedWith_SearchRef_ShouldCallRequest_WithSearchRef(ManageEvidenceOverviewDisplayOption selectedTab)
        {
            // arrange
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var evidenceNoteViewModel = Fixture.Create<ManageEvidenceNoteViewModel>();
            var searchRef = Fixture.Create<string>();
            evidenceNoteViewModel.FilterViewModel.SearchRef = searchRef;

            // act
            var result = await ManageEvidenceController.Index(organisationId, aatfId, selectedTab.ToDisplayString(), evidenceNoteViewModel);

            // assert
            A.CallTo(() => WeeeClient.SendAsync(A<string>._, A<GetAatfNotesRequest>.That.Matches(x => x.SearchRef == searchRef))).MustHaveHappenedOnceExactly();
        }
    }
}