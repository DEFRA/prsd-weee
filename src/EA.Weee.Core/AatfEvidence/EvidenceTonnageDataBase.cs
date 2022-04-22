namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using DataReturns;

    public class EvidenceTonnageDataBase
    {
        public decimal? Received { get; protected set; }

        public decimal? Reused { get; protected set; }

        public WeeeCategory CategoryId { get; protected set; }

        public Guid Id { get; protected set; }

        public EvidenceTonnageDataBase()
        {
        }
    }
}
