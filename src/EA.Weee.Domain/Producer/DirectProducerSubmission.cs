﻿namespace EA.Weee.Domain.Producer
{
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    
    public class DirectProducerSubmission : Entity
    {
        public DirectProducerSubmission()
        {
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
        }

        public virtual Guid DirectRegistrantId { get; set; }

        public virtual Guid? FinalPaymentSessionId { get; set; }

        public virtual Guid RegisteredProducerId { get; set; }
        
        public virtual bool PaymentFinished { get; set; }

        public virtual int ComplianceYear { get; set; }

        public virtual string ManualPaymentMethod { get; set; }

        public virtual DateTime? ManualPaymentReceivedDate { get; set; }

        public virtual string ManualPaymentDetails { get; set; }

        public virtual string ManualPaymentMadeByUserId { get; set; }

        public virtual User.User ManualPaymentMadeByUser { get; set; }

        public virtual DirectProducerSubmissionStatus DirectProducerSubmissionStatus { get; set; }

        public virtual DirectRegistrant DirectRegistrant { get; set; }

        public virtual RegisteredProducer RegisteredProducer { get; set; }

        public virtual DirectProducerSubmissionHistory CurrentSubmission { get; set; }

        public virtual ICollection<DirectProducerSubmissionHistory> SubmissionHistory { get; set; }

        public virtual PaymentSession FinalPaymentSession { get; set; }

        public void SetCurrentSubmission(DirectProducerSubmissionHistory submission)
        {
            Condition.Requires(submission).IsNotNull();

            CurrentSubmission = submission;
        }

        public void SetPaymentInformation(PaymentState paymentState)
        {
            Condition.Requires(paymentState).IsNotNull();
        }

        public DirectProducerSubmission(DirectRegistrant directRegistrant,
            RegisteredProducer registeredProducer,
            int complianceYear)
        {
            Condition.Requires(directRegistrant).IsNotNull();
            Condition.Requires(registeredProducer).IsNotNull();
            Condition.Ensures(complianceYear).IsGreaterThan(0);

            RegisteredProducer = registeredProducer;
            DirectRegistrant = directRegistrant;
            ComplianceYear = complianceYear;
            SubmissionHistory = new List<DirectProducerSubmissionHistory>();
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
        }

        public DirectProducerSubmission(RegisteredProducer registeredProducer, 
            int complianceYear)
        {
            Condition.Requires(registeredProducer).IsNotNull();
            Condition.Ensures(complianceYear).IsGreaterThan(0);

            RegisteredProducer = registeredProducer;
            ComplianceYear = complianceYear;
            SubmissionHistory = new List<DirectProducerSubmissionHistory>();
            DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Incomplete;
        }
    }
}
