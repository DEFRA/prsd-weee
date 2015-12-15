namespace EA.Weee.Requests.Admin
{
    using System;
    using Prsd.Core.Mediator;

    public class RemoveProducer : IRequest<int>
    {
        public Guid RegisteredProducerId { get; set; }

        public RemoveProducer(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
