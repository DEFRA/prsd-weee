namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System;

    public class SummaryViewModel
    {
        public double EstimatedCsvFileSizeInKilobytes { get; private set; }

        public Guid MemberUploadId { get; private set; }

        public static SummaryViewModel Create(double estimatedCsvFileSizeInKilobytes, Guid memberUploadId)
        {
            return new SummaryViewModel
            {
                EstimatedCsvFileSizeInKilobytes = estimatedCsvFileSizeInKilobytes,
                MemberUploadId = memberUploadId
            };
        }
    }
}