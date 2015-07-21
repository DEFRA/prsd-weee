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

        public override bool Equals(object obj)
        {
            var authorisedRepresentativeObj = obj as AuthorisedRepresentative;

            if (authorisedRepresentativeObj == null)
            {
                return false;
            }

            var compareOverseasContact = false;
            if (OverseasContact == null && authorisedRepresentativeObj.OverseasContact == null)
            {
                compareOverseasContact = true;
            }
            else
            {
                if (OverseasContact != null && authorisedRepresentativeObj.OverseasContact != null)
                {
                    compareOverseasContact =
                        OverseasContact.Equals(authorisedRepresentativeObj.OverseasContact);
                }
            }
            return OverseasProducerName.Equals(authorisedRepresentativeObj.OverseasProducerName) &&
                   compareOverseasContact;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string OverseasProducerName { get; private set; }

        public Guid? OverseasContactId { get; private set; }

        public virtual ProducerContact OverseasContact { get; private set; }
    }
}
