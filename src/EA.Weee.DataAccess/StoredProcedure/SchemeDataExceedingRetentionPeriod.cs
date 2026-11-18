namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SchemeDataExceedingRetentionPeriod
    {
        public string SchemeName { get; set; }

        public Guid SchemeId { get; set; }

        public string ApprovalNumber { get; set; }

        public int ComplianceYear { get; set; }
    }
}
