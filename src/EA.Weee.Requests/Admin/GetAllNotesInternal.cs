namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using AatfEvidence;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;

    public class GetAllNotesInternal : EvidenceNoteFilterBaseRequest
    {
        public GetAllNotesInternal(List<NoteType> noteTypeFilterList, 
            List<NoteStatus> allowedStatuses, 
            int complianceYear,
            int pageNumber,
            int pageSize)
        {
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(noteTypeFilterList);
            Condition.Requires(complianceYear).IsGreaterThan(0);
            Condition.Requires(pageNumber).IsGreaterThan(0);
            Condition.Requires(pageSize).IsGreaterThan(0);

            AllowedStatuses = allowedStatuses;
            NoteTypeFilterList = noteTypeFilterList;
            ComplianceYear = complianceYear;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}