namespace EA.Weee.Requests.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using Core.Admin.Obligation;
    using Prsd.Core.Mediator;

    public class GetSchemeObligationUpload : IRequest<List<SchemeObligationUploadErrorData>>
    {
        public Guid ObligationUploadId { get; }

        public GetSchemeObligationUpload(Guid obligationUploadId)
        {
            ObligationUploadId = obligationUploadId;
        }
    }
}
