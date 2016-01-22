namespace EA.Weee.Domain.Producer
{
    using Prsd.Core;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Domain;

    public class RegisteredProducer : Entity, IEquatable<RegisteredProducer>
    {
        public RegisteredProducer(
            string producerRegistrationNumber,
            int complianceYear,
            Scheme scheme)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            ProducerRegistrationNumber = producerRegistrationNumber;
            ComplianceYear = complianceYear;
            Scheme = scheme;
            CurrentSubmission = null;
            Removed = false;
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        protected RegisteredProducer()
        {
        }

        public virtual bool Removed { get; private set; }

        public virtual string ProducerRegistrationNumber { get; private set; }

        public virtual int ComplianceYear { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual ProducerSubmission CurrentSubmission { get; private set; }

        public virtual ICollection<ProducerSubmission> ProducerSubmissions { get; private set; }

        public void SetCurrentSubmission(ProducerSubmission producerSubmission)
        {
            Guard.ArgumentNotNull(() => producerSubmission, producerSubmission);

            if (producerSubmission.RegisteredProducer != this)
            {
                throw new InvalidOperationException();
            }
            CurrentSubmission = producerSubmission;
        }

        public void Remove()
        {
            if (Removed)
            {
                throw new InvalidOperationException("Cannot remove a producer which has previously been removed.");
            }

            Removed = true;
        }

        public virtual bool Equals(RegisteredProducer other)
        {
            if (other == null)
            {
                return false;
            }

            return ProducerRegistrationNumber == other.ProducerRegistrationNumber &&
                   ComplianceYear == other.ComplianceYear &&
                   Scheme.ApprovalNumber == other.Scheme.ApprovalNumber &&
                   Removed == other.Removed;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegisteredProducer);
        }

        public override int GetHashCode()
        {
            return ProducerRegistrationNumber.GetHashCode() ^ ComplianceYear.GetHashCode() ^
                   Scheme.ApprovalNumber.GetHashCode() ^ Removed.GetHashCode();
        }
    }
}
