namespace EA.Weee.RequestHandlers.DataReturns.Upload
{
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain.DataReturns;
    using EA.Prsd.Core.Mediator;
    using Requests.DataReturns;
    using Security;
    using QuarterType = Core.DataReturns.QuarterType;

    public class GetUploadInfoByDataReturnUploadIdHandler : IRequestHandler<GetUploadInfoByDataReturnUploadId, DataReturnUploadInfo>
    {
        private readonly IFetchDataReturnUploadDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public GetUploadInfoByDataReturnUploadIdHandler(IWeeeAuthorization authorization, IFetchDataReturnUploadDataAccess dataAccess)
        {   
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<DataReturnUploadInfo> HandleAsync(GetUploadInfoByDataReturnUploadId message)
        {   
            DataReturnUpload dataReturnUpload = await dataAccess.FetchDataReturnUploadByIdAsync(message.DataReturnUploadId);
            authorization.EnsureSchemeAccess(dataReturnUpload.Scheme.Id);

            var quarter = new DataReturnUploadInfo
            {
                Quarter = (QuarterType?)dataReturnUpload.Quarter,
                Year = dataReturnUpload.ComplianceYear
            };

            return quarter;
        }
    }
}
