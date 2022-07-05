namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using Core.Tests.Unit.Helpers;
    using Xunit;

    public class ViewTransferEvidenceNoteTonnageDataViewModelTests
    {
        private readonly Fixture fixture;

        public ViewTransferEvidenceNoteTonnageDataViewModelTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeTrue_IfNotesWithCategoriesPresentAndNoteIsDraft()
        {
            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel
            {
                Status = NoteStatus.Draft,
                CategoryValues = new List<EvidenceCategoryValue>() { fixture.Create<EvidenceCategoryValue>() }
            };

            // assert
            model.DisplayTransferNote.Should().BeTrue();
        }

        [Fact(Skip = "TODO: be fixed")]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeFalse_IfNoNotesWithCategoriesPresent()
        {
            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel
            {
                Status = NoteStatus.Draft,
                CategoryValues = new List<EvidenceCategoryValue>()
            };

            // assert
            model.DisplayTransferNote.Should().BeFalse();
        }

        [Fact(Skip = "TODO: be fixed")]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeFalse_IfNullCategoryValues()
        {
            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel
            {
                Status = NoteStatus.Draft,
                CategoryValues = null
            };

            // assert
            model.DisplayTransferNote.Should().BeFalse();
        }

        [Theory(Skip = "TODO: be fixed")]
        [ClassData(typeof(NoteStatusCoreData))]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeFalse_IfNotesWithCategoriesPresentAndNoteIsNotDraft(NoteStatus status)
        {
            if (status == NoteStatus.Draft)
            {
                return;
            }

            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel
            {
                Status = status,
                CategoryValues = new List<EvidenceCategoryValue>() { fixture.Create<EvidenceCategoryValue>() }
            };

            // assert
            model.DisplayTransferNote.Should().BeFalse();
        }
    }
}
