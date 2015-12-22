namespace EA.Weee.RequestHandlers.DataReturns.SubmitReturnVersion
{
    using System;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using FetchDataReturnForSubmission;
    using Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;

    internal class SubmitDataReturnUploadHandler : IRequestHandler<SubmitDataReturnUpload, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISubmitReturnVersionDataAccess submitReturnVersionDataAccess;
        private readonly IFetchDataReturnForSubmissionDataAccess fetchDataReturnForSubmissionDataAccess;

        public SubmitDataReturnUploadHandler(IWeeeAuthorization authorization,
            ISubmitReturnVersionDataAccess submitReturnVersionDataAccess,
            IFetchDataReturnForSubmissionDataAccess fetchDataReturnForSubmissionDataAccess)
        {
            this.authorization = authorization;
            this.submitReturnVersionDataAccess = submitReturnVersionDataAccess;
            this.fetchDataReturnForSubmissionDataAccess = fetchDataReturnForSubmissionDataAccess;
        }

        public async Task<Guid> HandleAsync(SubmitDataReturnUpload message)
        {
            DataReturnUpload dataReturnsUpload = await fetchDataReturnForSubmissionDataAccess.FetchDataReturnUploadAsync(message.DataReturnUploadId);

            authorization.EnsureSchemeAccess(dataReturnsUpload.Scheme.Id);

            var dataReturnVersion = dataReturnsUpload.DataReturnVersion;
            await submitReturnVersionDataAccess.Submit(dataReturnVersion);

            return dataReturnVersion.Id;
        }
    }
}
