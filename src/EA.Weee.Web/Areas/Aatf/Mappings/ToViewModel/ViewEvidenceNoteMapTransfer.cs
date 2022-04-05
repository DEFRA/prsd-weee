namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;

    public class ViewEvidenceNoteMapTransfer
    {
        public Guid OrganisationId { get; protected set; }
        public Guid AatfId { get; protected set; }

        public EvidenceNoteData EvidenceNoteData { get; private set; }

        public ViewEvidenceNoteMapTransfer(EvidenceNoteData evidenceNoteData)
        {
            EvidenceNoteData = evidenceNoteData;
        }
    }
}