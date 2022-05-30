namespace EA.Weee.Requests.Admin.Obligations
{
    using System;
    using Core.Admin.Obligation;
    using Prsd.Core.Mediator;

    public class GetSchemeObligationUpload : IRequest<SchemeObligationUploadData>
    {
        public Guid ObligationUploadId { get; private set; }

        public GetSchemeObligationUpload(Guid obligationUploadId)
        {
            ObligationUploadId = obligationUploadId;
        }
    }
}
