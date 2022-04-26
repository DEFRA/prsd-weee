﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class AllOtherEvidenceNotesViewModelTests
    {
        [Fact]
        public void AllOtherEvidenceNotesViewModel_Constructor_ShouldSetTab()
        {
            var model = new AllOtherEvidenceNotesViewModel();

            model.ActiveOverviewDisplayOption.Should().Be(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes);
        }

        [Fact]
        public void AllOtherEvidenceNotesViewModel_Constructor_ShouldInitialiseList()
        {
            var model = new AllOtherEvidenceNotesViewModel();

            model.EvidenceNotesDataList.Should().NotBeNull();
            model.EvidenceNotesDataList.Should().BeEmpty();
        }
    }
}
