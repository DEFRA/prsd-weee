namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Xunit;

    public class TransferredOutEvidenceNotesViewModelMapTransferTests
    {
        [Fact]
        public void TransferredOutEvidenceNotesViewModelMapTransferInheritsBaseEvidenceNotesViewModelMapTransfer()
        {
            typeof(TransferredOutEvidenceNotesViewModelMapTransfer).BaseType.Name.Should().Be(nameof(BaseEvidenceNotesViewModelMapTransfer));
        }
    }
}
