namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using Domain.DataReturns;
    using System;
    using System.Threading.Tasks;

    public interface IFetchDataReturnForSubmissionDataAccess
    {
        Task<DataReturnUpload> FetchDataReturnUploadAsync(Guid dataReturnsUploadId);

        Task<bool> CheckForExistingSubmissionAsync(Guid schemeId, int complianceYear, int quarterType);
    }
}