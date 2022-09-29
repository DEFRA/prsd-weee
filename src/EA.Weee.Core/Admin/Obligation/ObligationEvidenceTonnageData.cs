namespace EA.Weee.Core.Admin.Obligation
{
    using EA.Weee.Core.DataReturns;

    public class ObligationEvidenceTonnageData
    {
        public WeeeCategory CategoryId { get; protected set; }

        public decimal? Obligation { get; protected set; }

        public decimal? Evidence { get; protected set; }

        public decimal? Reuse { get; protected set; }

        public decimal? TransferredOut { get; protected set; }

        public decimal? TransferredIn { get; protected set; }

        public decimal? Difference { get; protected set; }

        public decimal? EvidenceOriginal { get; protected set; }

        public decimal? EvidenceDifference { get; protected set; }

        public ObligationEvidenceTonnageData(WeeeCategory categoryId, decimal? obligation, decimal? evidence, decimal? reuse, decimal? transferredOut, decimal? transferredIn, decimal? difference, decimal? evidenceOriginal, decimal? evidenceDifference)
        {
            CategoryId = categoryId;
            Obligation = obligation;
            Evidence = evidence;
            Reuse = reuse;
            TransferredOut = transferredOut;
            TransferredIn = transferredIn;
            Difference = difference;
            EvidenceOriginal = evidenceOriginal;
            EvidenceDifference = evidenceDifference;
        }
    }
}
