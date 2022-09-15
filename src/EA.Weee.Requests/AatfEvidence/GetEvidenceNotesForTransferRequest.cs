﻿namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesForTransferRequest : IRequest<EvidenceNoteSearchDataResult>
    {
        public Guid OrganisationId { get; private set; }

        public List<int> Categories { get; private set; }

        public List<Guid> ExcludeEvidenceNotes { get; private set; }

        public int ComplianceYear { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public int? Reference { get; private set; }

        public GetEvidenceNotesForTransferRequest(Guid organisationId, 
            List<int> categories, 
            int complianceYear,
            List<Guid> excludeEvidenceNotes,
            int? reference = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categories).IsNotEmpty();

            OrganisationId = organisationId;
            Categories = categories;
            ComplianceYear = complianceYear;
            ExcludeEvidenceNotes = excludeEvidenceNotes ?? new List<Guid>();
            Reference = reference;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
