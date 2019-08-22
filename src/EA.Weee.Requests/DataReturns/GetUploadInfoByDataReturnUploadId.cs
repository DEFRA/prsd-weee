namespace EA.Weee.Requests.DataReturns
{
    using Core.DataReturns;
    using Prsd.Core.Mediator;
    using System;

    public class GetUploadInfoByDataReturnUploadId : IRequest<DataReturnUploadInfo>
    {
        public Guid DataReturnUploadId { get; private set; }

        public GetUploadInfoByDataReturnUploadId(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
