namespace EA.Weee.RequestHandlers.Shared
{
    using Core.DataReturns;
    using Domain.DataReturns;

    public class DataReturnSubmissionsData : DataReturnSubmissionsHistoryData
    {
        public DataReturnVersion DataReturnVersion { get; set; }
    }
}
