namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;

    public class ListOfEvidenceNoteDataMap
    {
        public List<EvidenceNoteData> ListOfEvidenceNoteData;

        public ListOfEvidenceNoteDataMap()
        {
            ListOfEvidenceNoteData = new List<EvidenceNoteData>();
        }
    }
}
