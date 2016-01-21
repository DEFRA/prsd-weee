namespace EA.Weee.Requests.DataReturns
{
    using System;
    using Prsd.Core.Mediator;

    public class SubmitDataReturnUpload : IRequest<Guid>
    {
        public Guid DataReturnUploadId { get; private set; }

        public SubmitDataReturnUpload(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
