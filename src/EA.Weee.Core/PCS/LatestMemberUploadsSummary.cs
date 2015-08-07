namespace EA.Weee.Core.PCS
{
    using System;
    using System.Collections.Generic;

    public class LatestMemberUploadsSummary
    {
        public IEnumerable<LatestMemberUpload> LatestMemberUploads { get; set; }

        public LatestMemberUploadsSummary()
        {
            LatestMemberUploads = new List<LatestMemberUpload>();
        }
    }
}
