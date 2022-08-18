namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewEvidenceNotesMapTransferTests : SimpleUnitTestBase
    {
        private readonly EvidenceNoteSearchDataResult noteData;
        private readonly ManageEvidenceNoteViewModel manageEvidenceNoteViewModel;
        private const int PageNumber = 2;
        private const int PageSize = 3;

        public ViewEvidenceNotesMapTransferTests()
        {
            var evidenceNoteData1 = new EvidenceNoteData();
            var evidenceNoteData2 = new EvidenceNoteData();

            var notes = new List<EvidenceNoteData>() { evidenceNoteData1, evidenceNoteData2 };
            noteData = TestFixture.Build<EvidenceNoteSearchDataResult>()
                .With(e => e.Results, notes).Create();

            manageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_PropertiesShouldBeSet()
        {
            // arrange
            var complianceYearsList = TestFixture.CreateMany<int>();
            var currentDate = SystemTime.Now;
           
            //act
            var model = new ViewEvidenceNotesMapTransfer(noteData, manageEvidenceNoteViewModel, currentDate, PageNumber, PageSize, complianceYearsList);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(manageEvidenceNoteViewModel);
            model.NoteData.Should().Be(noteData);
            model.CurrentDate.Should().Be(currentDate);
            model.ComplianceYearList.Should().BeEquivalentTo(complianceYearsList);
            model.PageNumber.Should().Be(PageNumber);
            model.PageSize.Should().Be(PageSize);
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_NotesIsNull_ShouldThrowAnException()
        {
            //act
            var result = Record.Exception(() => new ViewEvidenceNotesMapTransfer(null,
                manageEvidenceNoteViewModel,
                SystemTime.Now, PageNumber, PageSize, TestFixture.CreateMany<int>()));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ViewAllEvidenceNotesMapModel_Constructor_GivenPageNumberIsLessThanOne_ShouldThrowAnException(int pageNumber)
        {
            //act
            var result = Record.Exception(() => new ViewEvidenceNotesMapTransfer(noteData,
                manageEvidenceNoteViewModel,
                SystemTime.Now,
                pageNumber,
                2,
                TestFixture.CreateMany<int>()));

            // assert
            result.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ViewAllEvidenceNotesMapModel_Constructor_GivenPageSizeIsLessThanOne_ShouldThrowAnException(int pageSize)
        {
            //act
            var result = Record.Exception(() => new ViewEvidenceNotesMapTransfer(noteData,
                manageEvidenceNoteViewModel,
                SystemTime.Now,
                1,
                pageSize,
                TestFixture.CreateMany<int>()));

            // assert
            result.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_ManageEvidenceNoteViewModelIsNull_PropertiesShouldBeSet()
        {
            // arrange 
            var currentDate = SystemTime.Now;

            //act
            var model = new ViewEvidenceNotesMapTransfer(noteData, 
                null, 
                currentDate,
                PageNumber,
                PageSize,
                TestFixture.CreateMany<int>());

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeNull();
            model.CurrentDate.Should().Be(currentDate);
            model.PageNumber.Should().Be(PageNumber);
            model.PageSize.Should().Be(PageSize);
        }
    }
}
