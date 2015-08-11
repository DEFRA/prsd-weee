﻿namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class AuthorisedRepresentative : Entity, IEquatable<AuthorisedRepresentative>
    {
        public AuthorisedRepresentative(string name, ProducerContact overseasContact = null)
        {
            OverseasProducerName = name;
            OverseasContact = overseasContact;
        }

        protected AuthorisedRepresentative()
        {
        }

        public bool Equals(AuthorisedRepresentative other)
        {
            if (other == null)
            {
                return false;
            }

            return OverseasProducerName == other.OverseasProducerName &&
                   object.Equals(OverseasContact, other.OverseasContact);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AuthorisedRepresentative);
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
