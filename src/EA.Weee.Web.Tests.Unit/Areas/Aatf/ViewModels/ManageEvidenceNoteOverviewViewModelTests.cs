namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class ManageEvidenceNoteOverviewViewModelTests
    {
        [Fact]
        public void ManageEvidenceNoteOverviewViewModel_ShouldBeAbstract()
        {
            typeof(ManageEvidenceNoteOverviewViewModel).Should().BeAbstract();
        }

        [Fact]
        public void ManageEvidenceNoteOverviewViewModel_SearchRef_ShouldHaveDisplayAttribute()
        {
            typeof(ManageEvidenceNoteOverviewViewModel)
                .GetProperty("SearchRef")
                .Should()
                .BeDecoratedWith<DisplayNameAttribute>(d => d.DisplayName.Equals("Search by reference ID"));
        }
    }
}
