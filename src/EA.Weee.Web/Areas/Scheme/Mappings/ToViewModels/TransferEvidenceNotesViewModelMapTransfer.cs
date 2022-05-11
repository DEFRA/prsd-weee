namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNotesViewModelMapTransfer
    {
        public IList<EvidenceNoteData> Notes { get; private set; }

        public TransferEvidenceNoteRequest Request { get; private set; }

        public Guid OrganisationId { get; private set; }

        public bool TransferAllTonnage { get; set; }

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