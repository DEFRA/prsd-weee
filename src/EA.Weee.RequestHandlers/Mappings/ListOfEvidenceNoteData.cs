namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;

    public class ListOfEvidenceNoteData
    {
        public List<EvidenceNoteData> ListOfEvidenceNoteData;

        public ListOfEvidenceNoteData()
        {
            ListOfEvidenceNoteData = new List<EvidenceNoteData>();
        }
    }
}
