namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using FluentAssertions;
    using Xunit;

    public class ManageEvidenceNotesTabDisplayOptionsTests
    {
        [Fact]
        public void ManageEvidenceNotesTabDisplayOptions_ShouldHaveCorrectDisplayAttributes()
        {
            //assert
            ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString().Should().Be("view-all-evidence-notes");
            ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString().Should().Be("view-all-evidence-transfers");
        }
    }
}
