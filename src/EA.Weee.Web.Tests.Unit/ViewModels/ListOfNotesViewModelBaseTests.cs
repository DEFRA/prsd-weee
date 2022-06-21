namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using FakeItEasy;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ListOfNotesViewModelBaseTests
    {
        private readonly IMapper mapper;
        private readonly TestListBase testClass;
        private readonly Fixture fixture;

        public ListOfNotesViewModelBaseTests()
        {
            mapper = A.Fake<IMapper>();

            testClass = new TestListBase(mapper);
            fixture = new Fixture();
        }

        private class TestListBase : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>
        {
            public TestListBase(IMapper mapper) : base(mapper)
            {
            }
        }

        [Fact]
        public void ListOfNotesViewModelBase_ShouldBeAbstract()
        {
            typeof(ListOfNotesViewModelBase<>).Should().BeAbstract();
        }

        [Fact]
        public void ListOfNotesViewModelBase_ShouldHaveIManageEvidenceViewModelAsT()
        {
            typeof(ListOfNotesViewModelBase<>).GetGenericArguments()[0].GetGenericParameterConstraints()[0].Name
                .Should().Be(nameof(IManageEvidenceViewModel));
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => testClass.Map(null, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>(),
                fixture.Create<EvidenceNoteData>()
            };

            //act
            testClass.Map(notes, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>());

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                fixture.Create<EvidenceNoteRowViewModel>(),
                fixture.Create<EvidenceNoteRowViewModel>(),
                fixture.Create<EvidenceNoteRowViewModel>()
            };

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = testClass.Map(notes, fixture.Create<DateTime>(), fixture.Create<ManageEvidenceNoteViewModel>());

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = testClass.Map(notes, date, model);

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
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(year, 1, 1);

            //act
            var result = testClass.Map(notes, date, null);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int complianceYear)
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = testClass.Map(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2022);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(2022, 1, 1);
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();

            //act
            var result = testClass.Map(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }
    }
}
