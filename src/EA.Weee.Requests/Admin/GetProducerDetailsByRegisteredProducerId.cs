namespace EA.Weee.Requests.Admin
{
    using System;
    using Core.Admin;
    using Prsd.Core.Mediator;

    public class GetProducerDetailsByRegisteredProducerId : IRequest<ProducerDetailsScheme>
    {
        public Guid RegisteredProducerId { get; set; }

        public GetProducerDetailsByRegisteredProducerId(Guid registeredProducerId)
        {
            RegisteredProducerId = registeredProducerId;
        }
    }
}
