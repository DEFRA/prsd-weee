namespace EA.Weee.RequestHandlers.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Collections.Generic;

    public class DataReturnSubmissionEeeChanges
    {
        public int ComplianceYear { get; set; }

        public int Quarter { get; set; }

        public string SchemeApprovalNumber { get; set; }

        public DateTime CurrentSubmissionDate { get; set; }

        public List<DataReturnSubmissionEeeChangesCsvData> CsvData { get; set; }
    }
}
