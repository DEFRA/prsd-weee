namespace EA.Weee.Core.Scheme
{
    using System.Collections.Generic;

    public class SchemeDownloadsByYears
    {
        public IList<SchemeDownloadsByYear> SchemeDownloads { get; set; }

        public SchemeDownloadsByYears()
        {
            SchemeDownloads = new List<SchemeDownloadsByYear>();
        }
    }
}
