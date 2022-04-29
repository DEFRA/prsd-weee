namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using System;
    using Core.AatfEvidence;
    using Prsd.Core;

    public class ViewEvidenceNoteMapTransfer
    {
        public Guid SchemeId { get; set; }

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