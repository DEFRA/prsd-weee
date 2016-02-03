namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides the data from the three result sets of the [PCS].[SpgSchemeWeeeCsv] stored procedure.
    /// </summary>
    public class SpgSchemeWeeeCsvResult
    {
        public class SchemeResult
        {
            public Guid SchemeId { get; set; }

            public string SchemeName { get; set; }

            public string ApprovalNumber { get; set; }
        }

        public class CollectedAmountResult
        {
            public Guid SchemeId { get; set; }

            public int QuarterType { get; set; }

            public int WeeeCategory { get; set; }

            public int SourceType { get; set; }

            public decimal Tonnage { get; set; }
        }

        public class DeliveredAmountResult
        {
            public Guid SchemeId { get; set; }

            public int QuarterType { get; set; }

            public int WeeeCategory { get; set; }

            public int LocationType { get; set; }

            public string LocationApprovalNumber { get; set; }

            public decimal Tonnage { get; set; }
        }

        public List<SchemeResult> Schemes { get; private set; }

        public List<CollectedAmountResult> CollectedAmounts { get; private set; }

        public List<DeliveredAmountResult> DeliveredAmounts { get; private set; }

        public SpgSchemeWeeeCsvResult()
        {
            Schemes = new List<SchemeResult>();
            CollectedAmounts = new List<CollectedAmountResult>();
            DeliveredAmounts = new List<DeliveredAmountResult>();
        }
    }
}
