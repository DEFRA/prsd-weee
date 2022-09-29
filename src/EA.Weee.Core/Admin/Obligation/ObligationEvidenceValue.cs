﻿namespace EA.Weee.Core.Admin.Obligation
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;

    public class ObligationEvidenceValue : CategoryValue
    {
        public virtual string Obligation { get; set; }
        public virtual string Evidence { get; set; }
        public virtual string Reused { get; set; }
        public virtual string TransferredOut { get; set; }
        public virtual string TransferredIn { get; set; }
        public virtual string Difference { get; set; }

        public virtual string EvidenceOriginal { get; set; }

        public virtual string EvidenceDifference { get; set; }

        public ObligationEvidenceValue(WeeeCategory categoryId) : base(categoryId)
        {
        }
    }
}
