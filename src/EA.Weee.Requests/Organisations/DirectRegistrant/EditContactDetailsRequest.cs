namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;
    using EA.Weee.Core.Organisations;

    public class EditContactDetailsRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; private set; }

        public AddressData AddressData {get; private set; }

        public ContactData ContactData { get; private set; }

        public EditContactDetailsRequest(Guid directRegistrantId, AddressData addressData, ContactData contactData)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(addressData).IsNotNull();
            Condition.Requires(contactData).IsNotNull();

            DirectRegistrantId = directRegistrantId;
            ContactData = contactData;
            AddressData = addressData;
        }
    }
}
