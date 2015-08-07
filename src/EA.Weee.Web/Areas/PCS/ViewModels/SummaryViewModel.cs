namespace EA.Weee.Web.Areas.PCS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.PCS;

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