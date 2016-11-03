namespace EA.Weee.RequestHandlers.Shared
{
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IDataReturnSubmissionsDataAccess
    {
        Task<DataReturnVersion> GetPreviousSubmission(DataReturnVersion dataReturnVersion);
    }
}
