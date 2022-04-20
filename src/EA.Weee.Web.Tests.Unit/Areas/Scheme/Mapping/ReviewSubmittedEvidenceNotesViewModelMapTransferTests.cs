namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Xunit;

    public class ReviewSubmittedEvidenceNotesViewModelMapTransferTests
    {
        [Fact]
        public void CheckReviewSubmittedEvidenceNotesViewModelMapTransferInheritsBaseEvidenceNotesViewModelMapTransfer()
        {
            typeof(ReviewSubmittedEvidenceNotesViewModelMapTransfer).BaseType.Name.Should().Be(nameof(BaseEvidenceNotesViewModelMapTransfer));
        }
    }
}
