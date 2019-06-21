namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;

    public class CompleteRegistration : IRequest<Guid>
    {
        public CompleteRegistration(Guid id, Guid addressId, Guid contactId)
        {
            OrganisationId = id;
            AddressId = addressId;
            ContactId = contactId;
        }

        public Guid OrganisationId { get; set; }

        public Guid ContactId { get; set; }

        public Guid AddressId { get; set; }
    }
}
