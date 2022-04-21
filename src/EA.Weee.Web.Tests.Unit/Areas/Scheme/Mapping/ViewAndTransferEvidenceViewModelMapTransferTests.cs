namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Xunit;

    public class ViewAndTransferEvidenceViewModelMapTransferTests
    {
        [Fact]
        public void CheckViewAndTransferEvidenceViewModelMapTransferInheritsBaseEvidenceNotesViewModelMapTransfer()
        {
            typeof(ViewAndTransferEvidenceViewModelMapTransfer).BaseType.Name.Should().Be(nameof(BaseEvidenceNotesViewModelMapTransfer));
        }
    }
}
