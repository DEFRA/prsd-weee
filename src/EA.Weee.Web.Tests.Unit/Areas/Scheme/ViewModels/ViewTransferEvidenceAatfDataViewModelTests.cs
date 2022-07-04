namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class ViewTransferEvidenceAatfDataViewModelTests
    {
        private readonly Fixture fixture;

        public ViewTransferEvidenceAatfDataViewModelTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewTransferEvidenceAatfDataViewModel_DisplayAatf_ShouldBeTrue_WhenThereAreNotesToBeDisplayed()
        {
            // arrange
            var model = new ViewTransferEvidenceAatfDataViewModel();
            var categoryValues = new List<EvidenceCategoryValue>() { fixture.Create<EvidenceCategoryValue>() };
            var notesList = new List<ViewTransferEvidenceNoteTonnageDataViewModel>();
            var note = new ViewTransferEvidenceNoteTonnageDataViewModel() { CategoryValues = categoryValues };
            notesList.Add(note);
            model.Notes = notesList;

            // assert
            model.DisplayAatf.Should().BeTrue();
        }

        [Fact]
        public void ViewTransferEvidenceAatfDataViewModel_DisplayAatf_ShouldBeFalse_WhenThereAreNoNotesToBeDisplayed()
        {
            // arrange
            var model = new ViewTransferEvidenceAatfDataViewModel();

            // assert
            model.DisplayAatf.Should().BeFalse();
        }

        [Fact]
        public void ViewTransferEvidenceAatfDataViewModel_DisplayAatf_ShouldBeFalse_WhenThereAreOnlyEmptyNotesToBeDisplayed()
        {
            // arrange
            var model = new ViewTransferEvidenceAatfDataViewModel();
            var notesList = new List<ViewTransferEvidenceNoteTonnageDataViewModel>();
            var note = new ViewTransferEvidenceNoteTonnageDataViewModel() { CategoryValues = new List<EvidenceCategoryValue>() };
            notesList.Add(note);
            model.Notes = notesList;

            // assert
            model.DisplayAatf.Should().BeFalse();
        }
    }
}
