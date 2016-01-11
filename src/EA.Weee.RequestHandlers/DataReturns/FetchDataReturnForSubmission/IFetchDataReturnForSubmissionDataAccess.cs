namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Scheme;

    public interface IFetchDataReturnForSubmissionDataAccess
    {
        Task<DataReturnUpload> FetchDataReturnUploadAsync(Guid dataReturnsUploadId);

        Task<bool> CheckForExistingSubmissionAsync(Guid schemeId, int complianceYear, int quarterType);
    }
}
