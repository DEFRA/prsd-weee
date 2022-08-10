namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewAllTransferNotesMapTests : SimpleUnitTestBase
    {
        private readonly ViewAllTransferNotesMap map;
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;
        private readonly DateTime currentDate;

        public ViewAllTransferNotesMapTests()
        {
            mapper = A.Fake<IMapper>();
            configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(25);

            map = new ViewAllTransferNotesMap(mapper, configurationService);

            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void ShouldBeDerivedFromViewAllEvidenceNotesMapBase()
        {
            typeof(ViewAllTransferNotesMap).Should().BeDerivedFrom<ListOfNotesViewModelBase<ViewAllTransferNotesViewModel>>();
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
        public void Map_GivenViewAllEvidenceNotesMapModelWithNoteData_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                null,
                SystemTime.Now,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>());

            //act
            map.Map(source);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e => e.SequenceEqual(noteData.Results)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenViewAllEvidenceNotesMapModelWithEmptyNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, new List<EvidenceNoteData>()).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                null,
                SystemTime.Now,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>());

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_PropertiesShouldBeMapped()
        {
            //arrange
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var source = new ViewEvidenceNotesMapTransfer(noteData, 
                managedEvidenceNoteViewModel,
                currentDate,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>());

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_PropertiesShouldBeMappedAsPagedList()
        {
            //arrange
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var pageNumber = TestFixture.Create<int>();
            var pageSize = TestFixture.Create<int>();

            var source = new ViewEvidenceNotesMapTransfer(noteData,
                managedEvidenceNoteViewModel,
                currentDate,
                pageNumber,
                TestFixture.CreateMany<int>());

            A.CallTo(() => configurationService.CurrentConfiguration.DefaultPagingPageSize).Returns(pageSize);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
            result.EvidenceNotesDataList.PageNumber.Should().Be(pageNumber);
            result.EvidenceNotesDataList.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            IEnumerable<int> complianceYearList = new List<int> { 2019, 2018, 2017 };
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2019, 1, 1);

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, date, TestFixture.Create<int>(), complianceYearList);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2019);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2018);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2017);
        }

        [Fact]
        public void Map_GivenCurrentDate_WhenGetComplianceYearListIsNull_ComplianceYearsListShouldBeReturned()
        {
            //arrange;
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, date, TestFixture.Create<int>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2022);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2021);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2020);
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2020)]
        [InlineData(2022)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelIsNull_SelectedComplianceYearShouldBeSet(int year)
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(year, 1, 1);

            var source = new ViewEvidenceNotesMapTransfer(noteData, null, date, TestFixture.Create<int>(), null);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2020)]
        [InlineData(2022)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelIsNullAndComplianceYearListIsProvided_SelectedComplianceYearShouldBeSet(int year)
        {
            //arrange
            var complianceYearList = new List<int>() { 2018, 2017 };
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(year, 1, 1);

            var source = new ViewEvidenceNotesMapTransfer(noteData, null, date, TestFixture.Create<int>(), complianceYearList);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2018);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZeroAndComplianceYearListIsNotProvided_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, currentDate, TestFixture.Create<int>(), new List<int>());

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZeroAndComplianceYearListIsProvided_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            var complianceYearList = new List<int>() { 2018, 2017 };
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, currentDate, TestFixture.Create<int>(), complianceYearList);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2018);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year - 1).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, currentDate, TestFixture.Create<int>(), new List<int>());

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year - 1);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYearAndComplianceYearListIsProvided_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year - 1).Create();

            var source = new ViewEvidenceNotesMapTransfer(noteData, model, currentDate, TestFixture.Create<int>(), new List<int>() { 2017});

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year - 1);
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenComplianceYearIsClosed_ComplianceYearClosedShouldBeTrue(DateTime currentDate, int complianceYear)
        {
            //arrange
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            var transfer = new ViewEvidenceNotesMapTransfer(
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                model,
                currentDate,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>().ToList());

            //act
            var result = map.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenComplianceYearIsNotClosed_ComplianceYearClosedShouldBeFalse()
        {
            //arrange
            var currentDate = new DateTime(2020, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year).Create();

            var transfer = new ViewEvidenceNotesMapTransfer(
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                model,
                currentDate,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>().ToList());

            //act
            var result = map.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
