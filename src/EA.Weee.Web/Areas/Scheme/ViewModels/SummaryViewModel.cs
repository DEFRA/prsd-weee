namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Scheme;

    public class SummaryViewModel
    {
        public IEnumerable<LatestMemberUploadViewModel> LatestMemberUploads { get; set; }

        public static SummaryViewModel Create(IEnumerable<LatestMemberUpload> latestMemberUploads)
        {
            return new SummaryViewModel
            {
                LatestMemberUploads = latestMemberUploads.Select(lmu => new LatestMemberUploadViewModel
                {
                    ComplianceYear = lmu.ComplianceYear,
                    CsvFileSizeEstimate = lmu.CsvFileSizeEstimate,
                    UploadId = lmu.UploadId
                })
            };
        }
    }
}