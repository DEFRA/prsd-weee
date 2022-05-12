namespace EA.Weee.Core.AatfEvidence
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using AatfReturn;
    using Organisations;
    using Prsd.Core;

    public class EvidenceNoteData
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        public WasteType? WasteType { get; set; }

        public Protocol? Protocol { get; set; }

        public int Reference { get; set; }

        public Guid RecipientId { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string ReturnedReason { get; set; }

        public List<EvidenceTonnageData> EvidenceTonnageData { get; set; }

        public SchemeData SchemeData { get; set; }

        public AatfData AatfData { get; set; }
        public OrganisationData OrganisationData { get; set; }

        public OrganisationData RecipientOrganisationData { get; set; }

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
