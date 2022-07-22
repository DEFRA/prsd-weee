namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Aatf;
    using Core.AatfEvidence;

    [Serializable]
    public class CreateEvidenceNoteRequest : EvidenceNoteBaseRequest
    {
        public CreateEvidenceNoteRequest()
        {
        }

        public CreateEvidenceNoteRequest(Guid organisationId, Guid aatfId, Guid recipientId, DateTime startDate, DateTime endDate, WasteType? wasteType, Protocol? protocol, List<TonnageValues> tonnages, NoteStatus status, Guid id) : base(organisationId, aatfId, recipientId, startDate, endDate, wasteType, protocol, tonnages, status, id)
        {
        }
    }
}
