namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using AatfReturn;

    [Serializable]
    public class TransferEvidenceNoteTonnageData
    {
        public AatfData OriginalAatf { get; set; }

        public NoteType Type { get; set; }

        public int Reference { get; set; }

        public EvidenceTonnageData EvidenceTonnageData { get; set; }
    }
}
