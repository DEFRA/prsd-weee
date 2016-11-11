namespace EA.Weee.RequestHandlers.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Threading.Tasks;

    public interface IGetDataReturnSubmissionEeeChangesCsvDataAccess
    {
        Task<DataReturnSubmissionEeeChanges> GetChanges(Guid currentDataReturnVersionId, Guid previousDataReturnVersionId);
    }
}