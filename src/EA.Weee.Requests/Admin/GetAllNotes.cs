namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;

    public class GetAllNotes : IRequest<List<EvidenceNoteData>>
    {
        public List<NoteType> NoteTypeFilterList { get; private set; }

        public List<NoteStatus> AllowedStatuses { get; set; }

        public GetAllNotes(List<NoteType> noteTypeFilterList, List<NoteStatus> allowedStatuses)
        {
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(noteTypeFilterList);

            AllowedStatuses = allowedStatuses;
            NoteTypeFilterList = noteTypeFilterList;
        }
    }
}