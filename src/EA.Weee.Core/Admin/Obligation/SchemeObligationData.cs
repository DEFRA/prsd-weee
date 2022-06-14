namespace EA.Weee.Core.Admin.Obligation
{
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;

    public class SchemeObligationData
    {
        public string SchemeName { get; private set; }

        public DateTime? UpdatedDate { get; private set; }

        public List<SchemeObligationAmountData> SchemeObligationAmountData { get; private set; }

        public SchemeObligationData(string schemeName,
            DateTime? updatedDate,
            List<SchemeObligationAmountData> schemeObligationAmountData)
        {
            Condition.Requires(schemeName).IsNotNullOrWhiteSpace();

            SchemeName = schemeName;
            UpdatedDate = updatedDate;
            SchemeObligationAmountData = schemeObligationAmountData;
        }
    }
}
