namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class AuthorisedRepresentative : Entity
    {
        public AuthorisedRepresentative(string name, ProducerContact overseasContact)
        {
            OverseasProducerName = name;
            OverseasContact = overseasContact;
        }

        protected AuthorisedRepresentative()
        {
        }

        public string OverseasProducerName { get; private set; }

        public virtual ProducerContact OverseasContact { get; private set; }
    }
}
