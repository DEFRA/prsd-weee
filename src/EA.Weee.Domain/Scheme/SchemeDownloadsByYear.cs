namespace EA.Weee.Domain.Scheme
{
    using Prsd.Core;

    public class SchemeDownloadsByYear
    {
        public int Year { get; private set; }

        public bool IsMembersDownloadAvailable { get; private set; }

        public bool IsDataReturnsDownloadAvailable { get; private set; }

        public SchemeDownloadsByYear(int year, bool isMembersDownloadAvailable = false, bool isDataReturnsDownloadAvailable = false)
        {
            Year = year;
            IsMembersDownloadAvailable = isMembersDownloadAvailable;
            IsDataReturnsDownloadAvailable = isDataReturnsDownloadAvailable;
        }
    }
}