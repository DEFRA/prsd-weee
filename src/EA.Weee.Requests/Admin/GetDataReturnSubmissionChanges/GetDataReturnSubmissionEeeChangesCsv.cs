namespace EA.Weee.Requests.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetDataReturnSubmissionEeeChangesCsv : IRequest<CSVFileData>
    {
        public Guid CurrentDataReturnVersionId { get; set; }

        public Guid PreviousDataReturnVersionId { get; set; }

        public GetDataReturnSubmissionEeeChangesCsv(Guid currentDataReturnVersionId, Guid previousDataReturnVersionId)
        {
            CurrentDataReturnVersionId = currentDataReturnVersionId;
            PreviousDataReturnVersionId = previousDataReturnVersionId;
        }
    }
}
