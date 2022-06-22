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

    public class EditDraftReturnNotesViewModelMapTests
    {
        private readonly EditDraftReturnNotesViewModelMap editDraftReturnNotesViewModelMap;
        private readonly Fixture fixture;
        private readonly IMapper mapper;
        private readonly DateTime currentDate;

        public EditDraftReturnNotesViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            editDraftReturnNotesViewModelMap = new EditDraftReturnNotesViewModelMap(mapper);

            fixture = new Fixture();

            currentDate = fixture.Create<DateTime>();
        }

        [Fact]
        public void EditDraftReturnNotesViewModelMap_ShouldBeDerivedFromListOfNotesViewModelBase()
        {
            typeof(EditDraftReturnNotesViewModelMap).Should()
                .BeDerivedFrom<ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>>();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNulLExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => editDraftReturnNotesViewModelMap.Map(null));

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
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate, model);

            //act
            editDraftReturnNotesViewModelMap.Map(transfer);

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
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate, model);

            //act
            editDraftReturnNotesViewModelMap.Map(transfer);

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
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate, model);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var model = fixture.Create<ManageEvidenceNoteViewModel>();
            var organisationId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var transfer = new EvidenceNotesViewModelTransfer(organisationId, aatfId, notes, currentDate, model);
            A.CallTo(() => mapper.Map<List<EvidenceNoteRowViewModel>>(notes)).Returns(returnedNotes);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var date = new DateTime(2019, 1, 1);
            var transfer = new EvidenceNotesViewModelTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), notes, date, model);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

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
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var date = new DateTime(year, 1, 1);
            var transfer = new EvidenceNotesViewModelTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), notes, date, null);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(year);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelSelectedComplianceYearIsNotGreaterThanZero_SelectedComplianceYearShouldBeSet(int selectedComplianceYear)
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, selectedComplianceYear).Create();
            var transfer = new EvidenceNotesViewModelTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), notes, currentDate, model);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year);
        }

        [Fact]
        public void Map_GivenCurrentDateAndManageEvidenceViewModelWithSelectedComplianceYear_SelectedComplianceYearShouldBeSet()
        {
            //arrange
            var notes = fixture.CreateMany<EvidenceNoteData>().ToList();
            var model = fixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, currentDate.Year - 1).Create();
            var transfer = new EvidenceNotesViewModelTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), notes, fixture.Create<DateTime>(), model);

            //act
            var result = editDraftReturnNotesViewModelMap.Map(transfer);

            //assert
            result.ManageEvidenceNoteViewModel.SelectedComplianceYear.Should().Be(currentDate.Year - 1);
        }
    }
}
