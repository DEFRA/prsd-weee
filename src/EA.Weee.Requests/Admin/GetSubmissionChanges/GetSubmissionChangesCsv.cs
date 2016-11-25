namespace EA.Weee.Requests.Admin.GetSubmissionChanges
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetSubmissionChangesCsv : IRequest<CSVFileData>
    {
        public Guid MemberUploadId { get; set; }

        public GetSubmissionChangesCsv(Guid memberUploadId)
        {
            MemberUploadId = memberUploadId;
        }
    }
}
