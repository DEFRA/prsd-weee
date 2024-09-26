namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;
    using EA.Weee.Core.Organisations;

    public class AddSignatoryAndCompleteRequest : IRequest<bool>
    {
        public Guid DirectRegistrantId { get; set; }

        public ContactData ContactData { get; set; }

        public AddSignatoryAndCompleteRequest(Guid directRegistrantId, ContactData contactData)
        {
            Condition.Requires(directRegistrantId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(contactData).IsNotNull();

            DirectRegistrantId = directRegistrantId;
            ContactData = contactData;
        }
    }
}
