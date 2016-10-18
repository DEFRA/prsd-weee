namespace EA.Weee.Core.DataReturns
{
    using System.Collections.Generic;

    public class DataReturnSubmissionsHistoryResult
    {
        public IList<DataReturnSubmissionsHistoryData> Data { get; set; }

        public int ResultCount { get; set; }

        public DataReturnSubmissionsHistoryResult()
        {
        }
    }
}
