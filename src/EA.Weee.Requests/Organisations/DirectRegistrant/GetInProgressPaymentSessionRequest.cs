namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    using System;
    public class GetInProgressPaymentSessionRequest : IRequest<SubmissionPaymentDetails>
    {
        public Guid DirectRegistrantId {get; set; }

        public GetInProgressPaymentSessionRequest(Guid directRegistrantId)
        {
            DirectRegistrantId = directRegistrantId;
        }
    }
}
