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
            currentSubmission = null;
        }

        /// <summary>
        /// This constructor should only be used by Entity Framework.
        /// </summary>
        protected RegisteredProducer()
        {
        }

        public string ProducerRegistrationNumber { get; private set; }

        public int ComplianceYear { get; private set; }

        public Scheme Scheme { get; private set; }

        private ProducerSubmission currentSubmission;
        public ProducerSubmission CurrentSubmission
        {
            get { return currentSubmission; }
            set
            {
                if (currentSubmission != null && value == null)
                {
                    string errorMessage = "Once a registration has been submitted, the current submission cannot be unset.";
                    throw new InvalidOperationException(errorMessage);
                }

                if (value != null && value.RegisteredProducer != this)
                {
                    string errorMessage = "The current producer submission for a registered producer can only be set to "
                        + "a submission that is already associated with the registered producer.";
                    throw new InvalidOperationException(errorMessage);
                }

                currentSubmission = value;
            }
        }
        public ICollection<ProducerSubmission> ProducerSubmissions { get; private set; }
    }
}
