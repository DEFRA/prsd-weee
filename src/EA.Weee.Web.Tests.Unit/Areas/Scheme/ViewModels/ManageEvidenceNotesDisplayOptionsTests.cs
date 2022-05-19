namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using Core.Helpers;
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;
    using Xunit;

    public class ManageEvidenceNotesDisplayOptionsTests
    {
        [Fact]
        public void ManageEvidenceOverviewDisplayOption_ShouldHaveCorrectDisplayAttributes()
        {
            //assert
            ManageEvidenceNotesDisplayOptions.Summary.ToDisplayString().Should().Be("evidence-summary");
            ManageEvidenceNotesDisplayOptions.ReviewSubmittedEvidence.ToDisplayString().Should().Be("review-submitted-evidence");
            ManageEvidenceNotesDisplayOptions.ViewAndTransferEvidence.ToDisplayString().Should().Be("view-and-transfer-evidence");
            ManageEvidenceNotesDisplayOptions.TransferredOut.ToDisplayString().Should().Be("transferred-out");
        }
    }
}
