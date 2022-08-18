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
    using Services;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ListOfNotesViewModelBaseTests : SimpleUnitTestBase
    {
        private readonly IMapper mapper;
        private readonly ConfigurationService configurationService;
        private readonly TestListBase testClass;

        public ListOfNotesViewModelBaseTests()
        {
            mapper = A.Fake<IMapper>();
            configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.DefaultInternalPagingPageSize).Returns(25);

            testClass = new TestListBase(mapper, configurationService);
        }

        private class TestListBase : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>
        {
            public TestListBase(IMapper mapper, ConfigurationService configurationService) : base(mapper, configurationService)
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
            var exception = Record.Exception(() => testClass.MapBase(null, TestFixture.Create<DateTime>(), TestFixture.Create<ManageEvidenceNoteViewModel>(), TestFixture.Create<int>()));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>
            {
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>(),
                TestFixture.Create<EvidenceNoteData>()
            };
            var noteData = new EvidenceNoteSearchDataResult(notes, 2);

            //act
            testClass.MapBase(noteData, TestFixture.Create<DateTime>(), TestFixture.Create<ManageEvidenceNoteViewModel>(), TestFixture.Create<int>());

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e => 
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNoteData_ShouldReturnMappedData()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>(),
                TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>._)).Returns(returnedNotes);

            //act
            var result = testClass.MapBase(noteData, TestFixture.Create<DateTime>(), TestFixture.Create<ManageEvidenceNoteViewModel>(), TestFixture.Create<int>());

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = testClass.MapBase(noteData, date, model, TestFixture.Create<int>());

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

            //act
            var result = testClass.MapBase(noteData, date, null, TestFixture.Create<int>());

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int complianceYear)
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = testClass.MapBase(noteData, date, model, TestFixture.Create<int>());

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2022);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();

            //act
            var result = testClass.MapBase(noteData, date, model, TestFixture.Create<int>());

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }

        [Fact]
        public void Map_GivenComplianceYearIsNotClosed_ComplianceYearClosedShouldBeFalse()
        {
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();

            //act
            var result = testClass.MapBase(noteData, date, model, TestFixture.Create<int>());

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(OutOfComplianceYearData))]
        public void Map_GivenComplianceYearIsClosed_ComplianceYearClosedShouldBeTrue(DateTime currentDate, int complianceYear)
        {
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = testClass.MapBase(noteData, currentDate, model, TestFixture.Create<int>());

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearClosed.Should().BeTrue();
        }
    }
}
