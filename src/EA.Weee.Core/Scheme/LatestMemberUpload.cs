namespace EA.Weee.Core.Scheme
{
    using System;

    public class LatestMemberUpload
    {
        public int ComplianceYear { get; set; }

        public Guid UploadId { get; set; }

        public double CsvFileSizeEstimate { get; set; }
    }
}
