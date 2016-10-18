namespace EA.Weee.Core.Admin
{
    using System.Collections.Generic;

    public class SubmissionsHistorySearchResult
    {
        public IList<SubmissionsHistorySearchData> Data { get; set; }

        public int ResultCount { get; set; }

        public SubmissionsHistorySearchResult()
        {
        }
    }
}
