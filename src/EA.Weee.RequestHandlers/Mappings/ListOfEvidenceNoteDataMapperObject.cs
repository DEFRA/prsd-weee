namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;

    public class ListOfEvidenceNoteDataMapperObject
    {
        public List<EvidenceNoteData> ListOfEvidenceNoteData;

        public ListOfEvidenceNoteDataMapperObject()
        {
            ListOfEvidenceNoteData = new List<EvidenceNoteData>();
        }
    }
}
