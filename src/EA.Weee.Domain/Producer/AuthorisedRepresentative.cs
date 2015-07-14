namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class AuthorisedRepresentative : Entity
    {
        public AuthorisedRepresentative(string name, Contact overseasContact)
        {
            OverseasProducerName = name;
            OverseasContact = overseasContact;
        }

        public string OverseasProducerName { get; private set; }

        public virtual Contact OverseasContact { get; private set; }
    }
}
