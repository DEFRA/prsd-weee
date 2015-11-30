namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RegisteredProducer : Entity
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
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        protected RegisteredProducer()
        {
        }

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
    }
}
