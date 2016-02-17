namespace EA.Weee.RequestHandlers.DataReturns.FetchDataReturnForSubmission
{
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain.DataReturns;
    using EA.Prsd.Core.Mediator;
    using Requests.DataReturns;
    using QuarterType = Core.DataReturns.QuarterType;

    public class GetQuarterInfoByDataReturnUploadIdHandler : IRequestHandler<GetQuarterInfoByDataReturnUploadId, QuarterInfo>
    {
        private readonly IFetchDataReturnForSubmissionDataAccess dataAccess;

        public GetQuarterInfoByDataReturnUploadIdHandler(IFetchDataReturnForSubmissionDataAccess dataAccess)
        {   
            this.dataAccess = dataAccess;
        }

        public async Task<QuarterInfo> HandleAsync(GetQuarterInfoByDataReturnUploadId message)
        {   
            DataReturnUpload dataReturnUpload = await dataAccess.FetchDataReturnUploadByIdAsync(message.DataReturnUploadId);

            var quarter = new QuarterInfo
            {
                Quarter = (QuarterType?)dataReturnUpload.Quarter,
                Year = dataReturnUpload.ComplianceYear
            };

            return quarter;
        }
    }
}
