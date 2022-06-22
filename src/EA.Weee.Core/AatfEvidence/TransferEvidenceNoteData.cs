namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.Shared;
    using Organisations;
    using Scheme;

    public class TransferEvidenceNoteData
    {
        public Guid Id { get; set; }

        public NoteType Type { get; set; }

        public NoteStatus Status { get; set; }

        public int Reference { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int ComplianceYear { get; set; }

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
