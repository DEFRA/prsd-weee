namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;

    public class ServiceOfNoticeRequest : IRequest<Guid>
    {
        public AddressData Address { get; private set; }

        public ServiceOfNoticeRequest(AddressData address)
        {
            Condition.Requires(address).IsNotNull();

            Address = address;
        }
    }
}
