namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;

    public class ViewEvidenceNoteMapTransfer
    {
        public EvidenceNoteData EvidenceNoteData { get; private set; }

        public object NoteStatus { get; private set; }

        public ViewEvidenceNoteMapTransfer(EvidenceNoteData evidenceNoteData, object noteStatus)
        {
            Guard.ArgumentNotNull(() => evidenceNoteData, evidenceNoteData);

            EvidenceNoteData = evidenceNoteData;
            NoteStatus = noteStatus;
        }
    }
}