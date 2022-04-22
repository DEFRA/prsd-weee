namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using EA.Prsd.Core;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetAatfSummaryRequest : IRequest<AatfEvidenceSummaryData>
    {
        public Guid AatfId { get; private set; }
        
        public GetAatfSummaryRequest(Guid aatfId)
        {
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            AatfId = aatfId;
        }
    }
}
