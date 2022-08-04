namespace EA.Weee.Core.AatfEvidence
{
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using AatfReturn;
    using Organisations;
    using Prsd.Core;

    public class EvidenceNoteData : EvidenceNoteDataBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Protocol? Protocol { get; set; }

        public Guid RecipientId { get; set; }

        public List<EvidenceTonnageData> EvidenceTonnageData { get; set; }

        public SchemeData RecipientSchemeData { get; set; }

        public AatfData AatfData { get; set; }

        public OrganisationData OrganisationData { get; set; }

        public SchemeData OrganisationSchemaData { get; set; }

        public OrganisationData RecipientOrganisationData { get; set; }

        public List<EvidenceNoteHistoryData> EvidenceNoteHistoryData { get; set; }

        public EvidenceNoteData()
        {
        }

        public EvidenceNoteData(SchemeData recipientSchemeData)
        {
            Guard.ArgumentNotNull(() => recipientSchemeData, recipientSchemeData);
            RecipientSchemeData = recipientSchemeData;
        }

        public EvidenceNoteData(SchemeData recipientSchemeData, AatfData aatfData)
        {
            Guard.ArgumentNotNull(() => aatfData, aatfData);
            Guard.ArgumentNotNull(() => recipientSchemeData, recipientSchemeData);

            RecipientSchemeData = recipientSchemeData;
            AatfData = aatfData;
        }
    }
}
