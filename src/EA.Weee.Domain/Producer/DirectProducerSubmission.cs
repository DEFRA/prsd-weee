namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;

    public class DirectProducerSubmission : Entity
    {
        public virtual Guid DirectRegistrantId { get; set; }

        public virtual Guid RegisteredProducerId { get; set; }

        public virtual int? PaymentStatus { get; set; }

        public virtual string PaymentReference { get; set; }

        public virtual string PaymentId { get; set; }

        public virtual int ComplianceYear {get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }

        public virtual RegisteredProducer RegisteredProducer { get; set; }

        public virtual DirectProducerSubmissionHistory CurrentSubmission { get; set; }

        public virtual ICollection<DirectProducerSubmissionHistory> SubmissionHistory { get; set; }

        public DirectProducerSubmission(DirectRegistrant directRegistrant, 
            RegisteredProducer registeredProducer, 
            int complianceYear,
            DirectProducerSubmissionHistory directProducerSubmissionHistory)
        {
            Condition.Requires(directRegistrant).IsNotNull();
            Condition.Requires(registeredProducer).IsNotNull();
            Condition.Requires(directProducerSubmissionHistory).IsNotNull();
            Condition.Ensures(complianceYear).IsGreaterThan(0);

            DirectRegistrant = directRegistrant;
            RegisteredProducer = registeredProducer;
            ComplianceYear = complianceYear;
        }
    }
}
