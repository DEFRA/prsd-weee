namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Xunit;

    public class SchemeTabViewModelMapTransferTests
    {
        [Fact]
        public void CheckViewAndTransferEvidenceViewModelMapTransferInheritsBaseEvidenceNotesViewModelMapTransfer()
        {
            typeof(SchemeTabViewModelMapTransfer).BaseType.Name.Should().Be(nameof(BaseEvidenceNotesViewModelMapTransfer));
        }
    }
}
