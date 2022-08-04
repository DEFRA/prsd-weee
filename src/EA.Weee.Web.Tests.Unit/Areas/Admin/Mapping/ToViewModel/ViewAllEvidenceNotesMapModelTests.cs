﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
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

    public class ViewAllEvidenceNotesMapModelTests : SimpleUnitTestBase
    {
        private readonly EvidenceNoteSearchDataResult noteData;
        private readonly ManageEvidenceNoteViewModel manageEvidenceNoteViewModel;

        public ViewAllEvidenceNotesMapModelTests()
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
            Func<IEnumerable<int>> func = () => new List<int>();
            var currentDate = SystemTime.Now;

            //act
            var model = new ViewAllEvidenceNotesMapTransfer(noteData, manageEvidenceNoteViewModel, currentDate, func);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(manageEvidenceNoteViewModel);
            model.NoteData.Should().Be(noteData);
            model.CurrentDate.Should().Be(currentDate);
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_NotesIsNull_ShouldThrowAnException()
        {
            //act
            Func<IEnumerable<int>> func = () => new List<int>();
            var result = Record.Exception(() => new ViewAllEvidenceNotesMapTransfer(null, manageEvidenceNoteViewModel, SystemTime.Now, func));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_ManageEvidenceNoteViewModelIsNull_PropertiesShouldBeSet()
        {
            // arrange 
            Func<IEnumerable<int>> func = () => new List<int>();
            var currentDate = SystemTime.Now;

            //act
            var model = new ViewAllEvidenceNotesMapTransfer(noteData, null, currentDate, func);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeNull();
            model.CurrentDate.Should().Be(currentDate);
        }
    }
}
