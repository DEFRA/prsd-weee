namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class AuthorisedRepresentative : Entity
    {
        public AuthorisedRepresentative(string name, ProducerContact overseasContact = null)
        {
            OverseasProducerName = name;
            OverseasContact = overseasContact;
        }

        protected AuthorisedRepresentative()
        {
        }

        public string OverseasProducerName { get; private set; }

        public Guid? OverseasContactId { get; private set; }
        public virtual ProducerContact OverseasContact { get; private set; }
    }
}
