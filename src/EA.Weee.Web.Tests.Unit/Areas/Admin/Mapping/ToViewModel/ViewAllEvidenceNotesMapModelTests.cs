namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using Xunit;

    public class ViewAllEvidenceNotesMapModelTests
    {
        private List<EvidenceNoteData> Notes { get; set; }
        private ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public ViewAllEvidenceNotesMapModelTests()
        {
            var evidenceNoteData1 = new EvidenceNoteData();
            var evidenceNoteData2 = new EvidenceNoteData();

            Notes = new List<EvidenceNoteData>() { evidenceNoteData1, evidenceNoteData2 };

            ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_PropertiesShouldBeSet()
        {
            //act
            var model = new ViewAllEvidenceNotesMapModel(Notes, ManageEvidenceNoteViewModel);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(ManageEvidenceNoteViewModel);
            model.Notes.SequenceEqual(Notes);
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_ConstructoR_NotesIsNull_ShouldThrowAnException()
        {
            //act
            var result = Record.Exception(() => new ViewAllEvidenceNotesMapModel(null, ManageEvidenceNoteViewModel));

            // assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_ConstructoR_NotesIsEmptyList_PropertiesShouldBeSet()
        {
            //act
            var model = new ViewAllEvidenceNotesMapModel(new List<EvidenceNoteData>(), ManageEvidenceNoteViewModel);

            //assert
            model.Should().NotBeNull();
            model.Notes.Should().NotBeNull();
            model.Notes.Should().BeEmpty();
        }

        [Fact]
        public void ViewAllEvidenceNotesMapModel_Constructor_ManageEvidenceNoteViewModelIsNull_PropertiesShouldBeSet()
        {
            //act
            var model = new ViewAllEvidenceNotesMapModel(Notes, null);

            //assert
            model.Should().NotBeNull();
            model.ManageEvidenceNoteViewModel.Should().BeNull();
        }
    }
}
