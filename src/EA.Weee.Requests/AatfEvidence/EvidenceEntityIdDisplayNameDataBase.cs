namespace EA.Weee.Requests.AatfEvidence
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class EvidenceEntityIdDisplayNameDataBase : IRequest<List<EntityIdDisplayNameData>>
    {
        public int ComplianceYear { get; }

        public List<NoteStatus> AllowedStatuses { get; }

        public EvidenceEntityIdDisplayNameDataBase(int complianceYear,
            List<NoteStatus> allowedStatuses)
        {
            Condition.Requires(allowedStatuses).IsNotNull();
            Condition.Requires(allowedStatuses).IsNotEmpty();
            Condition.Requires(complianceYear).IsGreaterThan(0);
            
            ComplianceYear = complianceYear;
            AllowedStatuses = allowedStatuses;
        }
    }
}
