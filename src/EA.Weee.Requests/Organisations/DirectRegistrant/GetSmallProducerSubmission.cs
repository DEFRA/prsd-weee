namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    using System;
    public class GetSmallProducerSubmission : IRequest<SmallProducerSubmissionData>
    {
        public Guid DirectRegistrantId {get; set; }

        public GetSmallProducerSubmission(Guid directRegistrantId)
        {
            DirectRegistrantId = directRegistrantId;
        }
    }
}
