namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using Core.AatfEvidence;
    using Prsd.Core;

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