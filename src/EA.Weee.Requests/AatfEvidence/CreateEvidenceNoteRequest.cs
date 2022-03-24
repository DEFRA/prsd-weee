namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Aatf;
    using Core.AatfEvidence;
    using Prsd.Core;

    public class CreateEvidenceNoteRequest : EvidenceNoteBaseRequest
    {
        public CreateEvidenceNoteRequest(Guid organisationId, 
            Guid aatfId,
            Guid recipientId,
            DateTime startDate, 
            DateTime endDate,
            WasteType wasteType,
            Protocol protocol,
            IList<TonnageValues> tonnages)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Guard.ArgumentNotDefaultValue(() => recipientId, recipientId);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);

            OrganisationId = organisationId;
            RecipientId = recipientId;
            AatfId = aatfId;
            StartDate = startDate;
            EndDate = endDate;
            WasteType = wasteType;
            Protocol = protocol;
            TonnageValues = tonnages;
        }
    }
}
