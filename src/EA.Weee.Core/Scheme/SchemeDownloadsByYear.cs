namespace EA.Weee.Core.Scheme
{
    public class SchemeDownloadsByYear
    {
        public int Year { get; set; }

        public bool IsMembersDownloadAvailable { get; set; }

        public bool IsDataReturnsDownloadAvailable { get; set; }

        public SchemeDownloadsByYear()
        {
            IsMembersDownloadAvailable = false;
            IsDataReturnsDownloadAvailable = false;
        }
    }
}