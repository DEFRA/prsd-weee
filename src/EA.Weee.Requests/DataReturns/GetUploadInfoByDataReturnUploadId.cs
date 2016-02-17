namespace EA.Weee.Requests.DataReturns
{
    using System;
    using Core.DataReturns;
    using Prsd.Core.Mediator;

    public class GetUploadInfoByDataReturnUploadId : IRequest<DataReturnUploadInfo>
    {
        public Guid DataReturnUploadId { get; private set; }

        public GetUploadInfoByDataReturnUploadId(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
