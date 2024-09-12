namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;

    public class ServiceOfNoticeRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; set; }

        public AddressData Address { get; private set; }

        public ServiceOfNoticeRequest(Guid directRegistrantId, AddressData address)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(address).IsNotNull();

            DirectRegistrantId = directRegistrantId;
            Address = address;
        }
    }
}
