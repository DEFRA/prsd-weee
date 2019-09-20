namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class RemoveProducer : IRequest<RemoveProducerResult>
    {
        public Guid RegisteredProducerId { get; set; }

        public RemoveProducer(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
