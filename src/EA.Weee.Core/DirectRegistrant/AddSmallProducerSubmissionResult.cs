namespace EA.Weee.Core.DirectRegistrant
{
    using System;

    public class AddSmallProducerSubmissionResult
    {
        public bool InvalidCache { get; private set; }

        public Guid SubmissionId { get; private set; }

        public AddSmallProducerSubmissionResult(bool invalidCache, Guid submissionId)
        {
            InvalidCache = invalidCache;
            SubmissionId = submissionId;
        }
    }
}
