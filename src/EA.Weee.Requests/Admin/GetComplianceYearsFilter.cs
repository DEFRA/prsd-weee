namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;

    public class GetComplianceYearsFilter : IRequest<IEnumerable<int>>
    {
        public List<NoteStatus> AllowedStatuses { get; set; }

        public GetComplianceYearsFilter(List<NoteStatus> allowedStatuses)
        {
            Guard.ArgumentNotNull(() => allowedStatuses, allowedStatuses);
            Condition.Requires(allowedStatuses).IsNotEmpty();

            AllowedStatuses = allowedStatuses;
        }
    }
}
