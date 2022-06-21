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
    using Xunit;

    public class AllOtherNotesViewModelMapTests
    {
        private readonly AllOtherNotesViewModelMap allOtherNotesViewModelMap;
        private readonly Fixture fixture;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public AllOtherNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            allOtherNotesViewModelMap = new AllOtherNotesViewModelMap(mapper);

            fixture = new Fixture();

            currentDate = fixture.Create<DateTime>();
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
            var notes = new List<EvidenceNoteData>
            {
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>(),
                 fixture.Create<EvidenceNoteData>()
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MapperShouldNotBeCalled()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);

            //act
            allOtherNotesViewModelMap.Map(transfer);

            // assert 
            A.CallTo(() => mapper.Map<EvidenceNoteRowViewModel>(A<EvidenceNoteRowViewModel>._)).MustHaveHappened(0, Times.Exactly);
        }

        [Fact]
        public void Map_GivenAnEmptyListOfEvidenceNotes_MustReturnAnEmptyModel()
        {
            //arrange
            var notes = new List<EvidenceNoteData>();

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert 
            result.EvidenceNotesDataList.Should().BeNullOrEmpty();
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

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

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
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].ApprovedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Approved;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenSubmittedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].SubmittedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Submitted;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenRejectedEvidenceNoteData_MappedDataDisplayViewLinkShouldBeTrue()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>(1).ToList();
            notes[0].RejectedDate = DateTime.Now;
            notes[0].Status = NoteStatus.Rejected;

            var returnedNotes = new List<EvidenceNoteRowViewModel>
            {
                 fixture.Create<EvidenceNoteRowViewModel>()
            };

            var model = new EditDraftReturnedNotesViewModel
            {
                EvidenceNotesDataList = returnedNotes
            };

            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = allOtherNotesViewModelMap.Map(transfer);

            // assert
            result.EvidenceNotesDataList[0].DisplayViewLink.Should().BeTrue();
        }

        [Fact]
        public void Map_GivenCurrentDate_ComplianceYearsListShouldBeReturned()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var date = new DateTime(2022, 1, 1);

            //act
            var result = allOtherNotesViewModelMap.Map(notes, date, model);

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
            var result = allOtherNotesViewModelMap.Map(notes, date, null);

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
            var model = fixture.Build<ManageEvidenceNoteViewModel>().With(m => m.SelectedComplianceYear, complianceYear).Create();

            //act
            var result = allOtherNotesViewModelMap.Map(notes, currentDate, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
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
            var result = allOtherNotesViewModelMap.Map(notes, date, model);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(2021);
        }
    }
}
