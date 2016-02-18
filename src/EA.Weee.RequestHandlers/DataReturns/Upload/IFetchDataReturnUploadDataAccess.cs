namespace EA.Weee.RequestHandlers.DataReturns.Upload
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;

    public interface IFetchDataReturnUploadDataAccess
    {  
        Task<DataReturnUpload> FetchDataReturnUploadByIdAsync(Guid dataReturnUploadId);
    }
}