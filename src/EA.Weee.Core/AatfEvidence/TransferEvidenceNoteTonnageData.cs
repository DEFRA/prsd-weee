namespace EA.Weee.Core.AatfEvidence
{
    using System;

    [Serializable]
    public class TransferEvidenceNoteTonnageData
    {
        public EvidenceTonnageData EvidenceTonnageData { get; set; }

        public EvidenceNoteData EvidenceNoteData { get; set; }
    }
}
