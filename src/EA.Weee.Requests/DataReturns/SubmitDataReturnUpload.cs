namespace EA.Weee.Requests.DataReturns
{
    using Prsd.Core.Mediator;
    using System;

    public class SubmitDataReturnUpload : IRequest<Guid>
    {
        public Guid DataReturnUploadId { get; private set; }

        public SubmitDataReturnUpload(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
