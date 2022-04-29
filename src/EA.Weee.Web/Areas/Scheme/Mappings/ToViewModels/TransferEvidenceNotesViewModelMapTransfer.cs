namespace EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;
    using Weee.Requests.Scheme;

    public class TransferEvidenceNotesViewModelMapTransfer
    {
        public List<EvidenceNoteData> Notes { get; private set; }

        public TransferEvidenceNoteRequest Request { get; private set; }

        public Guid OrganisationId { get; private set; }

        public TransferEvidenceNotesViewModelMapTransfer(List<EvidenceNoteData> notes,
            TransferEvidenceNoteRequest transferRequest,
            Guid organisation)
        {
            Notes = notes;
            Request = transferRequest;
            OrganisationId = organisation;
        }
    }
}