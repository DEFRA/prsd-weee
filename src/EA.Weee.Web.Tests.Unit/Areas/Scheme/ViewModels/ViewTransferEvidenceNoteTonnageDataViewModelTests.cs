namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class ViewTransferEvidenceNoteTonnageDataViewModelTests
    {
        private readonly Fixture fixture;

        public ViewTransferEvidenceNoteTonnageDataViewModelTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeTrue_IfNotesWithCategoriesPresent()
        {
            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel();
            model.CategoryValues = new List<EvidenceCategoryValue>() { fixture.Create<EvidenceCategoryValue>() };

            // assert
            model.DisplayTransferNote.Should().BeTrue();
        }

        [Fact]
        public void ViewTransferEvidenceNoteTonnageDataViewModel_DisplayTransferNote_ShouldBeFalse_IfNoNotesWithCategoriesPresent()
        {
            // arrange
            var model = new ViewTransferEvidenceNoteTonnageDataViewModel();
            model.CategoryValues = new List<EvidenceCategoryValue>();

            // assert
            model.DisplayTransferNote.Should().BeFalse();
        }
    }
}
