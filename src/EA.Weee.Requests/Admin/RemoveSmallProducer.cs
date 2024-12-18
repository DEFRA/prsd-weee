namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class RemoveSmallProducer : IRequest<RemoveSmallProducerResult>
    {
        public Guid RegisteredProducerId { get; set; }

        public RemoveSmallProducer(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
