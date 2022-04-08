namespace EA.Weee.Core.AatfEvidence
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AatfReturn;
    using Prsd.Core;

    public class EvidenceNoteData
    {
        //TODO: Other evidence note properties required
        public int Reference { get; set; }

        public Guid RecipientId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string SubmittedBy { get; set; }

        public Core.AatfEvidence.NoteStatus Status { get; set; }

        public Core.AatfEvidence.WasteType? WasteType { get; set; }

        public List<EvidenceTonnageData> EvidenceTonnageData { get; set; }

        public SchemeData SchemeData { get; set; }

        public AatfData AatfData { get; set; }

        public EvidenceNoteData()
        {
        }

        public EvidenceNoteData(SchemeData schemeData)
        {
            Guard.ArgumentNotNull(() => schemeData, schemeData);
            SchemeData = schemeData;
        }

        public EvidenceNoteData(SchemeData schemeData, AatfData aatfData)
        {
            Guard.ArgumentNotNull(() => aatfData, aatfData);
            Guard.ArgumentNotNull(() => schemeData, schemeData);

            SchemeData = schemeData;
            AatfData = aatfData;
        }
    }
}
