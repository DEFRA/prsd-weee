namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;

    using System;
    public class AddSmallProducerSubmission : IRequest<Guid>
    {
        public Guid DirectRegistrantId {get; set; }

        public AddSmallProducerSubmission(Guid directRegistrantId)
        {
            DirectRegistrantId = directRegistrantId;
        }
    }
}
