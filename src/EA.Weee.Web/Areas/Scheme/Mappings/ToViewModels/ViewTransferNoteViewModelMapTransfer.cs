namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;

    public class ViewTransferNoteViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; private set; }

        public object DisplayNotification { get; private set; }

        public int? SelectedComplianceYear { get; set; }

        public bool Edit { get; set; }

        public bool? ReturnToView { get; set; }

        public string RedirectTab { get; set; }

        public ViewTransferNoteViewModelMapTransfer(Guid organisationId, TransferEvidenceNoteData transferEvidenceNoteData, object displayNotification)
        {
            Condition.Requires(transferEvidenceNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            OrganisationId = organisationId;
            TransferEvidenceNoteData = transferEvidenceNoteData;
            DisplayNotification = displayNotification;
            SelectedComplianceYear = transferEvidenceNoteData.ComplianceYear;
        }
    }
}