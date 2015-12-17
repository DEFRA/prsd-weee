namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class RemoveProducer : IRequest<RemoveProducerResult>
    {
        public Guid RegisteredProducerId { get; set; }

        public RemoveProducer(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
