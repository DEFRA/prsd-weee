namespace EA.Weee.Domain.Scheme
{
    using System.Collections.Generic;
    using System.Linq;

    public class SchemeDownloadsByYears
    {
        public IList<SchemeDownloadsByYear> DownloadsByYears { get; private set; }

        private SchemeDownloadsByYears(IList<SchemeDownloadsByYear> downloadsByYear)
        {
            DownloadsByYears = downloadsByYear;
        }

        public static SchemeDownloadsByYears Create(IList<int> complianceYearsWithSubmittedMemberUploads,
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

            var downloadsByYear = new List<SchemeDownloadsByYear>();

            var years = complianceYearsWithSubmittedMemberUploads
                .Union(complianceYearsWithSubmittedDataReturns)
                .Distinct();

            foreach (var year in years)
            {
                downloadsByYear.Add(new SchemeDownloadsByYear(year,
                    complianceYearsWithSubmittedMemberUploads.Contains(year),
                    complianceYearsWithSubmittedDataReturns.Contains(year)));
            }

            return new SchemeDownloadsByYears(downloadsByYear);
        }
    }
}
