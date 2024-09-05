﻿namespace EA.Weee.Domain.Producer
{
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;
    using System;
    using System.Collections.Generic;

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

        public RegisteredProducer(
            string producerRegistrationNumber,
            int complianceYear)
        {
            ProducerRegistrationNumber = producerRegistrationNumber;
            ComplianceYear = complianceYear;
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

        public DateTime? RemovedDate { get; private set; }

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
            RemovedDate = SystemTime.UtcNow;
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
                   Removed == other.Removed &&
                   RemovedDate == other.RemovedDate;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegisteredProducer);
        }

        public override int GetHashCode()
        {
            var hashCode = ProducerRegistrationNumber.GetHashCode() ^ ComplianceYear.GetHashCode() ^
                Scheme.ApprovalNumber.GetHashCode() ^ Removed.GetHashCode();

            if (RemovedDate.HasValue)
            {
                hashCode ^= RemovedDate.Value.GetHashCode();
            }

            return hashCode;
        }
    }
}
