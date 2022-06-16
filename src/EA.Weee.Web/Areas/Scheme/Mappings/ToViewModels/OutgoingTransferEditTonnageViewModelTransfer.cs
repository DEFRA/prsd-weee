namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;

    public class OutgoingTransferEditTonnageViewModelTransfer : ViewTransferNoteViewModelMapTransfer
    {
        public OutgoingTransferEditTonnageViewModelTransfer(Guid schemeId, TransferEvidenceNoteData transferEvidenceNoteData) : base(schemeId, transferEvidenceNoteData, null)
        {
        }
    }
}