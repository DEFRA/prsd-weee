namespace EA.Weee.Requests.Admin
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetProducerDetailsByRegisteredProducerId : IRequest<ProducerDetailsScheme>
    {
        public Guid RegisteredProducerId { get; set; }

        public GetProducerDetailsByRegisteredProducerId(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
