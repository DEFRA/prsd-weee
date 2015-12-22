namespace EA.Weee.Requests.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
