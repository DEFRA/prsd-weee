namespace EA.Weee.Core.PCS
{
    using System;

    public class LatestMemberUpload
    {
        public int ComplianceYear { get; set; }

        public Guid UploadId { get; set; }

        public double CsvFileSizeEstimate { get; set; }
    }
}
