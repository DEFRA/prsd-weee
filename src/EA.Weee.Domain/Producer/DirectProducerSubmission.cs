namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Organisation;

    public class DirectProducerSubmission : Entity
    {
        public DirectProducerSubmission()
        {
        }

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

        public void SetCurrentSubmission(DirectProducerSubmissionHistory submission)
        {
            Condition.Requires(submission).IsNotNull();

            CurrentSubmission = submission;
        }

        public DirectProducerSubmission(DirectRegistrant directRegistrant,
            RegisteredProducer registeredProducer,
            int complianceYear)
        {
            Condition.Requires(directRegistrant).IsNotNull();
            Condition.Requires(registeredProducer).IsNotNull();
            Condition.Ensures(complianceYear).IsGreaterThan(0);

            RegisteredProducer = registeredProducer;
            ComplianceYear = complianceYear;
            SubmissionHistory = new List<DirectProducerSubmissionHistory>();
        }

        public DirectProducerSubmission(RegisteredProducer registeredProducer, 
            int complianceYear)
        {
            Condition.Requires(registeredProducer).IsNotNull();
            Condition.Ensures(complianceYear).IsGreaterThan(0);

            RegisteredProducer = registeredProducer;
            ComplianceYear = complianceYear;
            SubmissionHistory = new List<DirectProducerSubmissionHistory>();
        }
    }
}
