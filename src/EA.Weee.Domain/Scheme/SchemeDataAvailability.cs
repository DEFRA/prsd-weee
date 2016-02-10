namespace EA.Weee.Domain.Scheme
{
    using System.Collections.Generic;
    using System.Linq;

    public class SchemeDataAvailability
    {
        public IList<SchemeAnnualDataAvailability> DownloadsByYears { get; private set; }

        private SchemeDataAvailability(IList<SchemeAnnualDataAvailability> downloadsByYear)
        {
            DownloadsByYears = downloadsByYear;
        }

        public static SchemeDataAvailability Create(IList<int> complianceYearsWithSubmittedMemberUploads,
            IList<int> complianceYearsWithSubmittedDataReturns)
        {
            if (complianceYearsWithSubmittedMemberUploads == null)
            {
                complianceYearsWithSubmittedMemberUploads = new List<int>();
            }

            if (complianceYearsWithSubmittedDataReturns == null)
            {
                complianceYearsWithSubmittedDataReturns = new List<int>();
            }

            var downloadsByYear = new List<SchemeAnnualDataAvailability>();

            var years = complianceYearsWithSubmittedMemberUploads
                .Union(complianceYearsWithSubmittedDataReturns)
                .Distinct();

            foreach (var year in years)
            {
                downloadsByYear.Add(new SchemeAnnualDataAvailability(year,
                    complianceYearsWithSubmittedMemberUploads.Contains(year),
                    complianceYearsWithSubmittedDataReturns.Contains(year)));
            }

            return new SchemeDataAvailability(downloadsByYear);
        }
    }
}
