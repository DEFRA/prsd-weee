namespace EA.Weee.Requests.DataReturns
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;

    public class FetchDataReturnForSubmission : IRequest<DataReturnForSubmission>
    {
        public Guid DataReturnUploadId { get; private set; }

        public FetchDataReturnForSubmission(Guid dataReturnUploadId)
        {
            DataReturnUploadId = dataReturnUploadId;
        }
    }
}
