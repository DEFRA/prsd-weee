namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Web.ViewModels.Shared;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class AllOtherNotesViewModelMapTests : SimpleUnitTestBase
    {
        private readonly AllOtherNotesViewModelMap allOtherNotesViewModelMap;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public AllOtherNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            allOtherNotesViewModelMap = new AllOtherNotesViewModelMap(mapper);

            currentDate = TestFixture.Create<DateTime>();
        }

        [Fact]
        public void AllOtherNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(AllOtherNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<AllOtherManageEvidenceNotesViewModel>>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => allOtherNotesViewModelMap.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListOfEvidenceNotes_MapperShouldBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
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
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList.Should().NotBeEmpty();
            result.EvidenceNotesDataList.Should().BeEquivalentTo(returnedNotes);
        }

        [Fact]
        public void Map_GivenApprovedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].ApprovedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Approved;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].SubmittedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Submitted;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);

            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenRejectedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = TestFixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].RejectedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Rejected;
            var noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 TestFixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, noteData, currentDate, model);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(A<List<EvidenceNoteData>>.That.Matches(e =>
                e.SequenceEqual(noteData.Results.ToList())))).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2019, 1, 1);
            var model = TestFixture.Create<ManageEvidenceNoteViewModel>();
            var source = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                noteData, date, model);

            //act
            var result = allOtherNotesViewModelMap.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.ComplianceYearList.Count().Should().Be(3);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(0).Should().Be(2019);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(1).Should().Be(2018);
            result.ManageEvidenceNoteViewModel.ComplianceYearList.ElementAt(2).Should().Be(2017);
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
            var source = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                noteData, date, null);

            //act
            var result = allOtherNotesViewModelMap.Map(source);

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
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>().With(m => m.SelectedComplianceYear, complianceYear).Create();
            var source = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                noteData, currentDate, model);

            //act
            var result = allOtherNotesViewModelMap.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var noteData = TestFixture.Create<EvidenceNoteSearchDataResult>();
            var date = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2021).Create();
            var source = new EvidenceNotesViewModelTransfer(TestFixture.Create<Guid>(), TestFixture.Create<Guid>(),
                noteData, date, model);

            //act
            var result = allOtherNotesViewModelMap.Map(source);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }
    }
}
