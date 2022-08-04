﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewAllEvidenceNotesMapTests : SimpleUnitTestBase
    {
        private readonly ViewAllEvidenceNotesMap map;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public ViewAllEvidenceNotesMapTests()
        {
            mapper = A.Fake<IMapper>();

            map = new ViewAllEvidenceNotesMap(mapper);

            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void ShouldBeDerivedFromViewAllEvidenceNotesMapBase()
        {
            typeof(ViewAllEvidenceNotesMap).Should().BeDerivedFrom<ListOfNotesViewModelBase<ViewAllEvidenceNotesViewModel>>();
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
            Func<IEnumerable<int>> func = () => new List<int>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var source = new ViewAllEvidenceNotesMapTransfer(noteData, null, SystemTime.Now, func);

            //act
            map.Map(source);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e => e.SequenceEqual(noteData.Results)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenViewAllEvidenceNotesMapModelWithEmptyNotes_MustReturnAnEmptyModel()
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int>();
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, new List<EvidenceNoteData>()).Create();

            var source = new ViewAllEvidenceNotesMapTransfer(noteData, null, SystemTime.Now, func);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_PropertiesShouldBeMapped()
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int>();
            var managedEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>(),
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var source = new ViewAllEvidenceNotesMapTransfer(noteData, managedEvidenceNoteViewModel, currentDate, func);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = map.Map(source);

            // assert 
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int> { 2019, 2018, 2017};
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2019, 1, 1);
            var source = new ViewAllEvidenceNotesMapTransfer(noteData, model, date, func);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2019);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2018);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2017);
        }

        [Fact]
        public void Map_GivenCurrentDate_WhenGetComplianceYearFilterFuncIsNull_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            Func<IEnumerable<int>> func = null;
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2021, 1, 1);
            var source = new ViewAllEvidenceNotesMapTransfer(noteData, model, date, func);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2021);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2020);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2019);
        }

        [Theory]
        [InlineData(2021)]
        [InlineData(2020)]
        [InlineData(2022)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelIsNull_SelectedComplianceYearShouldBeSet(int year)
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(year, 1, 1);
            var source = new ViewAllEvidenceNotesMapTransfer(noteData, null, date, func);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();

            var source = new ViewAllEvidenceNotesMapTransfer(noteData, model, currentDate, func);

            //act
            var result = map.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            Func<IEnumerable<int>> func = () => new List<int>();
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year - 1).Create();

            var source = new ViewAllEvidenceNotesMapTransfer(noteData, model, currentDate, func);

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
            
            var transfer = new ViewAllEvidenceNotesMapTransfer(
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                model,
                currentDate,
                TestFixture.Create<Func<List<int>>>());

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

            var transfer = new ViewAllEvidenceNotesMapTransfer(
                TestFixture.Create<EvidenceNoteSearchDataResult>(),
                model,
                currentDate,
                TestFixture.Create<Func<List<int>>>());

            //act
            var result = map.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }
    }
}
