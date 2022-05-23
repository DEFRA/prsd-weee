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
            typeof(ManageManageEvidenceNoteOverviewViewModel).Should().BeAbstract();
        }
    }
}
