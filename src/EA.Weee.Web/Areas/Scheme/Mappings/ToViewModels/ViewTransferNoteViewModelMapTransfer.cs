namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using System.Security.Principal;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;

    public class ViewTransferNoteViewModelMapTransfer
    {
        public Guid OrganisationId { get; set; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; private set; }

        public object DisplayNotification { get; private set; }

        public bool Edit { get; set; }

        public bool? ReturnToView { get; set; }

        public string RedirectTab { get; set; }

        public DateTime SystemDateTime { get; set; }

        public IPrincipal User { get; private set; }

        public ViewTransferNoteViewModelMapTransfer(Guid organisationId, TransferEvidenceNoteData transferEvidenceNoteData,
            object displayNotification, IPrincipal user = null)
        {
            Condition.Requires(transferEvidenceNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            OrganisationId = organisationId;
            TransferEvidenceNoteData = transferEvidenceNoteData;
            DisplayNotification = displayNotification;
            User = user;
        }
    }
}