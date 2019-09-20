namespace EA.Weee.RequestHandlers.DataReturns
{
    using Domain.DataReturns;
    using System.Threading.Tasks;

    public interface IDataReturnSubmissionsDataAccess
    {
        Task<DataReturnVersion> GetPreviousSubmission(DataReturnVersion dataReturnVersion);
    }
}
