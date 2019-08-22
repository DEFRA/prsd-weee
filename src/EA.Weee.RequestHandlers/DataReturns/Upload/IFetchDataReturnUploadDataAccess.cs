namespace EA.Weee.RequestHandlers.DataReturns.Upload
{
    using Domain.DataReturns;
    using System;
    using System.Threading.Tasks;

    public interface IFetchDataReturnUploadDataAccess
    {
        Task<DataReturnUpload> FetchDataReturnUploadByIdAsync(Guid dataReturnUploadId);
    }
}