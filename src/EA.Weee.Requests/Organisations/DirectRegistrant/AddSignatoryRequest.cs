namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;
    using EA.Weee.Core.Organisations;

    public class AddSignatoryRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public ContactData ContactData { get; private set; }

        public AddSignatoryRequest(Guid directRegistrantId, ContactData contactData)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(contactData).IsNotNull();

            DirectRegistrantId = directRegistrantId;
            ContactData = contactData;
        }
    }
}
