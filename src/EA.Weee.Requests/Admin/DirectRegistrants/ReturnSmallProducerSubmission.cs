namespace EA.Weee.Requests.Admin.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class ReturnSmallProducerSubmission : IRequest<Guid>
    {
        public Guid DirectProducerSubmissionId { get; private set; }

        public ReturnSmallProducerSubmission(Guid directProducerSubmissionId)
        {
            DirectProducerSubmissionId = directProducerSubmissionId;
        }
    }
}
