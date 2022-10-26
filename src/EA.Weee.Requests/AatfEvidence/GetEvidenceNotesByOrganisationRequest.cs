﻿namespace EA.Weee.Requests.AatfEvidence
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    public class GetEvidenceNotesByOrganisationRequest : EvidenceNoteFilterBaseRequest
    {
        public NoteStatus? NoteStatusFilter { get; set; }

        public GetEvidenceNotesByOrganisationRequest(Guid organisationId, 
            List<NoteStatus> allowedStatuses,
            int complianceYear,
            List<NoteType> noteTypeFilterList,
            bool transferredOut,
            int pageNumber,
            int pageSize,
            NoteStatus? noteStatus,
            string searchRef)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(noteTypeFilterList);
            Condition.Requires(pageNumber).IsGreaterThan(0);
            Condition.Requires(pageSize).IsGreaterThan(0);

            OrganisationId = organisationId;
            AllowedStatuses = allowedStatuses;
            ComplianceYear = complianceYear;
            NoteTypeFilterList = noteTypeFilterList;
            TransferredOut = transferredOut;
            PageNumber = pageNumber;
            PageSize = pageSize;
            NoteStatusFilter = noteStatus;
            SearchRef = searchRef;
        }
    }
}
