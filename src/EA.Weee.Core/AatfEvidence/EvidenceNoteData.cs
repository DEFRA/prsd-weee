namespace EA.Weee.Core.AatfEvidence
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EvidenceNoteData
    {
        //TODO: Other evidence note properties required
        public int Reference { get; set; }

        public List<EvidenceTonnageData> EvidenceTonnageData { get; set; }

        public SchemeData SchemeData { get; private set; }

        public EvidenceNoteData(SchemeData schemeData)
        {
            SchemeData = schemeData;
        }
    }
}
