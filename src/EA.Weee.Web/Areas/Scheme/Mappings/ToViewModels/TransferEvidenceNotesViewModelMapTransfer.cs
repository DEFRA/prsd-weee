namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNotesViewModelMapTransfer
    {
        public IList<EvidenceNoteData> Notes { get; }

        public TransferEvidenceNoteRequest Request { get; }

        public TransferEvidenceNoteData TransferEvidenceNoteData { get; }

        public Guid OrganisationId { get; }

        public bool TransferAllTonnage { get; set; }

        public TransferEvidenceNotesViewModelMapTransfer(IList<EvidenceNoteData> notes,
            TransferEvidenceNoteData transferNoteData,
            Guid organisationId)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(transferNoteData).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Notes = notes;
            TransferEvidenceNoteData = transferNoteData;
            OrganisationId = organisationId;
        }

        public TransferEvidenceNotesViewModelMapTransfer(IList<EvidenceNoteData> notes,
            TransferEvidenceNoteRequest request,
            Guid organisationId)
        {
            Condition.Requires(notes).IsNotNull();
            Condition.Requires(request).IsNotNull();
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);

            Notes = notes;
            Request = request;
            OrganisationId = organisationId;
        }
    }
}