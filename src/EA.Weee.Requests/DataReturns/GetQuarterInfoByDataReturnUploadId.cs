namespace EA.Weee.Requests.DataReturns
{
    using System;
    using Core.DataReturns;
    using Prsd.Core.Mediator;

    public class GetQuarterInfoByDataReturnUploadId : IRequest<QuarterInfo>
    {
        public Guid DataReturnUploadId { get; private set; }

        public GetQuarterInfoByDataReturnUploadId(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
