namespace EA.Weee.Core.Scheme
{
    using System.Collections.Generic;

    public class LatestMemberUploadList
    {
        public IEnumerable<LatestMemberUpload> LatestMemberUploads { get; set; }

        public LatestMemberUploadList()
        {
            LatestMemberUploads = new List<LatestMemberUpload>();
        }
    }
}
