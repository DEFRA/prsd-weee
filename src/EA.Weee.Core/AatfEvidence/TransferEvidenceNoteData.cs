namespace EA.Weee.Core.AatfEvidence
{
    using Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Organisations;

    public class TransferEvidenceNoteData : EvidenceNoteDataBase
    {
        public List<TransferEvidenceNoteTonnageData> TransferEvidenceNoteTonnageData { get; set; }

        public List<int> CategoryIds
        {
            get
            {
                if (TransferEvidenceNoteTonnageData != null)
                {
                    return TransferEvidenceNoteTonnageData.Select(t => t.EvidenceTonnageData.CategoryId).Distinct().Cast<int>()
                        .ToList();
                }

                return new List<int>();
            }
        }
        public SchemeData RecipientSchemeData { get; set; }

        public OrganisationData TransferredOrganisationData { get; set; }

        public SchemeData TransferredSchemeData { get; set; }

        public OrganisationData RecipientOrganisationData { get; set; }

        public TransferEvidenceNoteData()
        {
        }
    }
}
