namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes;

    public class ViewTransferNoteViewModelMapTransfer
    {
        public Guid SchemeId { get; set; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; private set; }

        public object DisplayNotification { get; private set; }

        public int? SelectedComplianceYear { get; set; }

        public bool Edit { get; set; }

        public bool? ReturnToView { get; set; }

        public string RedirectTab { get; set; }

        public ViewTransferNoteViewModelMapTransfer(Guid schemeId, TransferEvidenceNoteData transferEvidenceNoteData, object displayNotification)
        {
            Condition.Requires(transferEvidenceNoteData).IsNotNull();
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);

            SchemeId = schemeId;
            TransferEvidenceNoteData = transferEvidenceNoteData;
            DisplayNotification = displayNotification;
            SelectedComplianceYear = transferEvidenceNoteData.ComplianceYear;
        }
    }
}