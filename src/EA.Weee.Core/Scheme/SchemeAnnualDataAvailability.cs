namespace EA.Weee.Core.Scheme
{
    public class SchemeAnnualDataAvailability
    {
        public int Year { get; set; }

        public bool IsMembersDownloadAvailable { get; set; }

        public bool IsDataReturnsDownloadAvailable { get; set; }

        public SchemeAnnualDataAvailability()
        {
            IsMembersDownloadAvailable = false;
            IsDataReturnsDownloadAvailable = false;
        }
    }
}