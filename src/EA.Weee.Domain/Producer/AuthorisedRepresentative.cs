﻿namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;
    using System;

    public class AuthorisedRepresentative : Entity, IEquatable<AuthorisedRepresentative>
    {
        public AuthorisedRepresentative(string name, ProducerContact overseasContact = null)
        {
            OverseasProducerName = name;
            OverseasContact = overseasContact;

            if (overseasContact != null)
            {
                if (!overseasContact.IsOverseas)
                {
                    string errorMessage = "The overseas producer of an authorised representative cannot be based in the UK.";
                    throw new ArgumentException(errorMessage);
                }
            }
        }

        public AuthorisedRepresentative(string name, string tradingName, ProducerContact overseasContact = null)
        {
            OverseasProducerName = name;
            OverseasProducerTradingName = tradingName;
            OverseasContact = overseasContact;
        }

        protected AuthorisedRepresentative()
        {
        }

        public virtual bool Equals(AuthorisedRepresentative other)
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

        public string OverseasProducerTradingName { get; private set; }

        public Guid? OverseasContactId { get; private set; }

        public virtual ProducerContact OverseasContact { get; private set; }

        public AuthorisedRepresentative OverwriteWhereNull(AuthorisedRepresentative other)
        {
            if (other == null)
            {
                return this;
            }

            OverseasContact = other.OverseasContact.OverwriteWhereNull(OverseasContact);
            other.OverseasContactId = OverseasContactId;
            other.OverseasProducerName = OverseasProducerName;
            other.OverseasProducerTradingName = OverseasProducerTradingName;

            return other;
        }
    }
}
