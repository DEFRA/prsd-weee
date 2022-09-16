namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.ViewModels
{
    using FluentAssertions;
    using Web.Areas.Scheme.ViewModels;
    using Xunit;

    public class TransferDeselectEvidenceNoteModelTests
    {
        [Fact]
        public void TransferDeselectEvidenceNoteModel_ShouldDeriveFromTransferSelectEvidenceNoteModelBase()
        {
            typeof(TransferDeselectEvidenceNoteModel).Should().BeDerivedFrom<TransferSelectEvidenceNoteModelBase>();
        }
    }
}
