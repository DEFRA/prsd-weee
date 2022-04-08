namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;

    public class ViewEvidenceNoteMapTransfer
    {
        public EvidenceNoteData EvidenceNoteData { get; }

        public ViewEvidenceNoteMapTransfer(EvidenceNoteData evidenceNoteData)
        {
            Guard.ArgumentNotNull(() => evidenceNoteData, evidenceNoteData);

            EvidenceNoteData = evidenceNoteData;
        }
    }
}