namespace EA.Weee.RequestHandlers.DataReturns
{
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IDataReturnSubmissionsDataAccess
    {
        Task<DataReturnVersion> GetPreviousSubmission(DataReturnVersion dataReturnVersion);
    }
}
