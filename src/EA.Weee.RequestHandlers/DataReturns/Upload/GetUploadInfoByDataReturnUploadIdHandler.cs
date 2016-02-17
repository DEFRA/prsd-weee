namespace EA.Weee.RequestHandlers.DataReturns.Upload
{
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain.DataReturns;
    using EA.Prsd.Core.Mediator;
    using Requests.DataReturns;
    using QuarterType = Core.DataReturns.QuarterType;

    public class GetUploadInfoByDataReturnUploadIdHandler : IRequestHandler<GetUploadInfoByDataReturnUploadId, DataReturnUploadInfo>
    {
        private readonly IFetchDataReturnUploadDataAccess dataAccess;

        public GetUploadInfoByDataReturnUploadIdHandler(IFetchDataReturnUploadDataAccess dataAccess)
        {   
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnUploadInfo> HandleAsync(GetUploadInfoByDataReturnUploadId message)
        {   
            DataReturnUpload dataReturnUpload = await dataAccess.FetchDataReturnUploadByIdAsync(message.DataReturnUploadId);

            var quarter = new DataReturnUploadInfo
            {
                Quarter = (QuarterType?)dataReturnUpload.Quarter,
                Year = dataReturnUpload.ComplianceYear
            };

            return quarter;
        }
    }
}
