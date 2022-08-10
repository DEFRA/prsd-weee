namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
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
            var model = new Web.Areas.Admin.Mappings.ToViewModel.ViewEvidenceNotesMapTransfer(noteData, manageEvidenceNoteViewModel, currentDate, TestFixture.Create<int>(), complianceYearsList);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(manageEvidenceNoteViewModel);
            model.NoteData.Should().Be(noteData);
            model.CurrentDate.Should().Be(currentDate);
            model.ComplianceYearList.Should().BeEquivalentTo(complianceYearsList);
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_NotesIsNull_ShouldThrowAnException()
        {
            //act
            var result = Record.Exception(() => new Web.Areas.Admin.Mappings.ToViewModel.ViewEvidenceNotesMapTransfer(null,
                manageEvidenceNoteViewModel,
                SystemTime.Now,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>()));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_ManageEvidenceNoteViewModelIsNull_PropertiesShouldBeSet()
        {
            // arrange 
            var currentDate = SystemTime.Now;

            //act
            var model = new Web.Areas.Admin.Mappings.ToViewModel.ViewEvidenceNotesMapTransfer(noteData, 
                null, 
                currentDate,
                TestFixture.Create<int>(),
                TestFixture.CreateMany<int>());

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeNull();
            model.CurrentDate.Should().Be(currentDate);
        }
    }
}
