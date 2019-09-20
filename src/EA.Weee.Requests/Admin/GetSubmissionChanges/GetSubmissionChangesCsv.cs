namespace EA.Weee.Requests.Admin.GetSubmissionChanges
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetSubmissionChangesCsv : IRequest<CSVFileData>
    {
        public Guid MemberUploadId { get; set; }

        public GetSubmissionChangesCsv(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
