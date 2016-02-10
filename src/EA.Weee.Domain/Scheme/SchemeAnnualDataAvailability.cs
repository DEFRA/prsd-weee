namespace EA.Weee.Domain.Scheme
{
    public class SchemeAnnualDataAvailability
    {
        public int Year { get; private set; }

        public bool IsMembersDownloadAvailable { get; private set; }

        public bool IsDataReturnsDownloadAvailable { get; private set; }

        public SchemeAnnualDataAvailability(int year, bool isMembersDownloadAvailable = false, bool isDataReturnsDownloadAvailable = false)
        {
            Year = year;
            IsMembersDownloadAvailable = isMembersDownloadAvailable;
            IsDataReturnsDownloadAvailable = isDataReturnsDownloadAvailable;
        }
    }
}