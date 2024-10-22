namespace EA.Weee.Requests.Admin.DirectRegistrants
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
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
