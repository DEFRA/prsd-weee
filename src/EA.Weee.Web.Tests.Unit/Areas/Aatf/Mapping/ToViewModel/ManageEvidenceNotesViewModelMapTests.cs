﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Scheme;
    using Web.Areas.Aatf.Helpers;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class ManageEvidenceNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly ManageEvidenceNoteViewModelMap map;
        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private readonly AatfData aatfData;
        private readonly List<AatfData> aatfDataList;
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;
        private readonly ConfigurationService configurationService;

        public ManageEvidenceNotesViewModelMapTests()
        {
            aatfEvidenceHelper = A.Fake<IAatfEvidenceHelper>();
            configurationService = A.Fake<ConfigurationService>();

            map = new ManageEvidenceNoteViewModelMap(aatfEvidenceHelper, configurationService);
            organisationId = Guid.NewGuid();
            aatfId = Guid.NewGuid();
            aatfData = TestFixture.Create<AatfData>();
            aatfDataList = TestFixture.CreateMany<AatfData>().ToList();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_ValidModelShouldBeReturned()
        {
            //arrange
            var source = TestFixture.Create<ManageEvidenceNoteTransfer>();

            //act
            var model = map.Map(source);

            //assert
            model.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSourceWithAatfDetails_PropertiesShouldBeMapped()
        {
            //arrange
            var aatfDataList = new List<AatfData>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.AatfId.Should().Be(source.AatfId);
            model.AatfName.Should().Be(source.AatfData.Name);
            model.OrganisationId.Should().Be(source.OrganisationId);
            model.AatfId.Should().Be(source.AatfId);
        }

        [Fact]
        public void Map_GivenSourceWithNoAatfList_AatfListPropertiesShouldBeSet()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.AatfName.Should().BeNullOrEmpty();
            model.SingleAatf.Should().BeFalse();
            model.CanCreateEdit.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithNoAatfData_AatfListPropertiesShouldBeSet()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.AatfName.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void Map_GivenSource_AatfHelperShouldBeCalled()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), TestFixture.Create<AatfData>(), TestFixture.CreateMany<AatfData>().ToList(),
                null, null, null, TestFixture.Create<int>(), DateTime.UtcNow);

            //act
            map.Map(source);

            // assert 
            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(source.Aatfs)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenSourceWithSingle_SingleAatfShouldBeTrue()
        {
            //arrange
            var listOfAatfs = TestFixture.CreateMany<AatfData>(1).ToList();
            var source = new ManageEvidenceNoteTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(), TestFixture.Create<AatfData>(), listOfAatfs,
             null, null, null, TestFixture.Create<int>(), DateTime.UtcNow);

            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(A<List<AatfData>>._))
                .Returns(listOfAatfs);

            //act
            var model = map.Map(source);

            // assert 
            model.SingleAatf.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSourceWithMultipleAatfs_SingleAatfShouldBeFalse()
        {
            //arrange
            var source = TestFixture.Create<ManageEvidenceNoteTransfer>();

            A.CallTo(() => aatfEvidenceHelper.GroupedValidAatfs(A<List<AatfData>>._))
                .Returns(TestFixture.CreateMany<AatfData>().ToList());

            //act
            var model = map.Map(source);

            // assert 
            model.SingleAatf.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceWithNullFilterModel_MapperShouldBeCalled()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.FilterViewModel.Should().BeOfType<FilterViewModel>();
            model.FilterViewModel.Should().BeEquivalentTo(new FilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithNullRecipientWasteStatusModel_MapperShouldBeCalled()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.RecipientWasteStatusFilterViewModel.Should().BeOfType<RecipientWasteStatusFilterViewModel>();
            model.RecipientWasteStatusFilterViewModel.Should().BeEquivalentTo(new RecipientWasteStatusFilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithNullSubmittedDatesModel_MapperShouldBeCalled()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.SubmittedDatesFilterViewModel.Should().BeOfType<SubmittedDatesFilterViewModel>();
            model.SubmittedDatesFilterViewModel.Should().BeEquivalentTo(new SubmittedDatesFilterViewModel());
        }

        [Fact]
        public void Map_GivenSourceWithValidFilterModel_MapperShouldBeCalled()
        {
            //arrange
            var aatfDataList = TestFixture.CreateMany<AatfData>().ToList();
            var filterModel = TestFixture.Create<FilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, filterModel, null, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.FilterViewModel.Should().Be(source.FilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidRecipientWasteStatusModel_MapperShouldBeCalled()
        {
            //arrange
            var aatfDataList = TestFixture.CreateMany<AatfData>().ToList();
            var recipientWasteStatusModel = TestFixture.Create<RecipientWasteStatusFilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, recipientWasteStatusModel, null, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.RecipientWasteStatusFilterViewModel.Should().Be(source.RecipientWasteStatusFilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceWithValidModel_MapperShouldBeCalled()
        {
            //arrange
            var aatfDataList = TestFixture.CreateMany<AatfData>().ToList();
            var submittedDateModel = TestFixture.Create<SubmittedDatesFilterViewModel>();
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, submittedDateModel, SystemTime.UtcNow.Year, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.SubmittedDatesFilterViewModel.Should().Be(source.SubmittedDatesFilterViewModel);
        }

        [Fact]
        public void Map_GivenSourceToMapper_ComplianceYearShouldBeMapped()
        {
            //arrange
            var expectedComplianceYear = TestFixture.Create<int>();
            var aatfDataList = new List<AatfData>
            {
                TestFixture.Build<AatfData>().With(ad => ad.FacilityType, FacilityType.Aatf).With(ad => ad.ComplianceYear, expectedComplianceYear).Create()
            };
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfDataList, null, null, null, expectedComplianceYear, SystemTime.UtcNow);

            //act
            var model = map.Map(source);

            // assert 
            model.SelectedComplianceYear.Should().Be(source.ComplianceYear);
        }

        [Fact]
        public void Map_GivenSourceWithCurrentDate_ComplianceYearListShouldBeMapped()
        {
            //arrange
            var currentDate = new DateTime(2019, 1, 1);
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, TestFixture.CreateMany<AatfData>().ToList(), null, null, null, TestFixture.Create<int>(), currentDate);

            var evidenceNoteStartDate = currentDate.AddYears(-2);
            A.CallTo(() => configurationService.CurrentConfiguration.EvidenceNotesSiteSelectionDateFrom)
                .Returns(evidenceNoteStartDate);

            //act
            var model = map.Map(source);

            // assert 
            model.ComplianceYearList.Should().BeEquivalentTo(new List<int>() { 2019, 2018, 2017 });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_GivenAatfCreateEvidenceNotesAndInComplianceYear_CanCreateEditShouldBeSet(bool canCreateEdit)
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();
            var aatfId = TestFixture.Create<Guid>();
            var currentDate = new DateTime(2021, 01, 31);
            var complianceYear = currentDate.Year;

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfs, null, null, null, complianceYear, currentDate);

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(A<List<AatfData>>.That.IsSameAs(aatfs), aatfId, complianceYear)).Returns(canCreateEdit);

            //act
            var result = map.Map(source);

            //assert
            result.CanCreateEdit.Should().Be(canCreateEdit);
        }

        [Fact]
        public void Map_GivenAatfCreateEvidenceNotesAndInComplianceYear_CanCreateEditShouldBeTrue()
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();
            var aatfId = TestFixture.Create<Guid>();
            var currentDate = new DateTime(2021, 01, 31);
            var complianceYear = currentDate.Year;

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfs, null, null, null, complianceYear, currentDate);

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(A<List<AatfData>>.That.IsSameAs(aatfs), aatfId, complianceYear)).Returns(true);

            //act
            var result = map.Map(source);

            //assert
            result.CanCreateEdit.Should().Be(true);
        }

        public static IEnumerable<object[]> ClosedComplianceDates =>
            new List<object[]>
            {
                new object[] { 2020, new DateTime(2021, 02, 1) },
                new object[] { 2019, new DateTime(2021, 02, 1) },
                new object[] { 2023, new DateTime(2021, 02, 1) }
            };

        [Theory]
        [MemberData(nameof(ClosedComplianceDates))]
        public void Map_GivenAatfCreateEvidenceNotesAndOutOfComplianceYear_CanCreateEditShouldBeSet(int complianceYear, DateTime currentDate)
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();
            var aatfId = TestFixture.Create<Guid>();

            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, aatfs, null, null, null, complianceYear, currentDate);

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(A<List<AatfData>>.That.IsSameAs(aatfs), aatfId, complianceYear)).Returns(true);

            //act
            var result = map.Map(source);

            //assert
            result.CanCreateEdit.Should().Be(false);
        }

        [Theory]
        [MemberData(nameof(ClosedComplianceDates))]
        public void Map_GivenOutOfComplianceYear_ComplianceYearClosedShouldBeTrue(int complianceYear, DateTime currentDate)
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, new List<AatfData>(), null, null, null, complianceYear, currentDate);

            //act
            var result = map.Map(source);

            //assert
            result.ComplianceYearClosed.Should().BeTrue();
        }

        public static IEnumerable<object[]> OpenComplianceDates =>
            new List<object[]>
            {
                new object[] { 2020, new DateTime(2021, 01, 31) },
                new object[] { 2020, new DateTime(2020, 12, 31) },
                new object[] { 2020, new DateTime(2020, 1, 1) },
            };

        [Theory]
        [MemberData(nameof(OpenComplianceDates))]
        public void Map_GivenInComplianceYear_ComplianceYearClosedShouldBeFalse(int complianceYear, DateTime currentDate)
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, new List<AatfData>(), null, null, null, complianceYear, currentDate);

            //act
            var result = map.Map(source);

            //assert
            result.ComplianceYearClosed.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenNullFilters_FiltersShouldBeInitialised()
        {
            //arrange
            var source = new ManageEvidenceNoteTransfer(organisationId, aatfId, aatfData, new List<AatfData>(), null, null, null, TestFixture.Create<int>(), TestFixture.Create<DateTime>());

            //act
            var result = map.Map(source);

            //assert
            result.FilterViewModel.Should().NotBeNull();
            result.RecipientWasteStatusFilterViewModel.Should().NotBeNull();
            result.SubmittedDatesFilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void Map_GivenSourceWithComplianceYearsList_ComplianceYearsShouldBeSet()
        {
            //arrange
            var list = TestFixture.CreateMany<int>().ToList();
            var source = new ManageEvidenceNoteTransfer(null, null, null, TestFixture.Create<int>(),
                TestFixture.Create<DateTime>(), list);

            //act
            var result = map.Map(source);

            //assert
            result.ComplianceYearList.Should().BeEquivalentTo(list);
        }
    }
}
