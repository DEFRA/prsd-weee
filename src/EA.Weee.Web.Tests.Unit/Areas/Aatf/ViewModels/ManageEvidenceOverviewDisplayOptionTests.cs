namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using Core.Helpers;
    using FluentAssertions;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class ManageEvidenceOverviewDisplayOptionTests
    {
        [Fact]
        public void ManageEvidenceOverviewDisplayOption_ShouldHaveCorrectDisplayAttributes()
        {
            //assert
            ManageEvidenceOverviewDisplayOption.EvidenceSummary.ToDisplayString().Should().Be("evidence-summary");
            ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes.ToDisplayString().Should().Be("edit-draft-and-returned-notes");
            ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString().Should().Be("view-all-other-evidence-notes");
        }
    }
}
