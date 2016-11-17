namespace EA.Weee.Domain.Scheme
{
    using System;
    using System.Collections.Generic;
    using Audit;
    using Charges;
    using Events;
    using Organisation;
    using Producer;
    using Prsd.Core;
    using User;
    using System.Linq;
    using Error;

    public class MemberUpload : AuditableEntity
    {
        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public virtual List<MemberUploadError> Errors { get; private set; }

        public virtual int? ComplianceYear { get; private set; }

        public virtual bool IsSubmitted { get; private set; }

        public virtual List<ProducerSubmission> ProducerSubmissions { get; private set; }

        public virtual decimal TotalCharges { get; private set; }

        public virtual string FileName { get; private set; }

        public virtual TimeSpan ProcessTime { get; private set; }

        public virtual MemberUploadRawData RawData { get; set; }

        public virtual InvoiceRun InvoiceRun { get; private set; }

        public virtual User SubmittedByUser { get; private set; }

        public DateTime? SubmittedDate { get; private set; }

        public MemberUpload(
            Guid organisationId,
            string data,
            List<MemberUploadError> errors,
            decimal totalCharges,
            int? complianceYear,
            Scheme scheme,
            string fileName,
            string userId = null)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            OrganisationId = organisationId;
            Scheme = scheme;
            Errors = errors;
            IsSubmitted = false;
            TotalCharges = totalCharges;
            ComplianceYear = complianceYear;
            RawData = new MemberUploadRawData() { Data = data };
            CreatedById = userId;
            FileName = fileName;
            ProducerSubmissions = new List<ProducerSubmission>();
        }

        public MemberUpload(
            Guid organisationId,
            Scheme scheme,
            string data,
            string fileName)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);

            OrganisationId = organisationId;
            Scheme = scheme;
            Errors = new List<MemberUploadError>();
            RawData = new MemberUploadRawData() { Data = data };
            FileName = fileName;
            ProducerSubmissions = new List<ProducerSubmission>();
        }

        public void Submit(User submittingUser)
        {
            Guard.ArgumentNotNull(() => submittingUser, submittingUser);

            if (IsSubmitted)
            {
                throw new InvalidOperationException("IsSubmitted status must be false to transition to true");
            }

            if (Errors != null &&
                Errors.Any(e => e.ErrorLevel == ErrorLevel.Error))
            {
                throw new InvalidOperationException("A member upload cannot be submitted when it contains errors");
            }

            IsSubmitted = true;
            SubmittedByUser = submittingUser;
            SubmittedDate = SystemTime.UtcNow;

            foreach (ProducerSubmission producerSubmission in ProducerSubmissions)
            {
                producerSubmission.RegisteredProducer.SetCurrentSubmission(producerSubmission);
            }

            RaiseEvent(new SchemeMemberSubmissionEvent(this));
        }

        public void DeductFromTotalCharges(decimal amount)
        {
            TotalCharges -= amount;
        }

        protected MemberUpload()
        {
        }

        public virtual void SetProcessTime(TimeSpan processTime)
        {
            if (ProcessTime.Ticks.Equals(0))
            {
                ProcessTime = processTime;
            }
            else
            {
                throw new InvalidOperationException("ProcessTime cannot be set for a MemberUpload that has already been given a ProcessTime value.");
            }
        }

        /// <summary>
        /// Assigns the member upload to the specified invoice run.
        /// </summary>
        /// <param name="invoiceRun"></param>
        internal void AssignToInvoiceRun(InvoiceRun invoiceRun)
        {
            Guard.ArgumentNotNull(() => invoiceRun, invoiceRun);

            if (InvoiceRun != null)
            {
                string errorMessage = "Once a member upload has been assigned to an invoice run, it cannot be reassigned.";
                throw new InvalidOperationException(errorMessage);
            }

            if (!IsSubmitted)
            {
                string errorMessage = "An unsubmitted member upload cannot be assigned to an invoice run.";
                throw new InvalidOperationException(errorMessage);
            }

            InvoiceRun = invoiceRun;

            foreach (var producer in ProducerSubmissions)
            {
                if (!producer.RegisteredProducer.Removed)
                {
                    producer.SetAsInvoiced();
                }
            }
        }

        public virtual int GetNumberOfWarnings()
        {
            return Errors == null ?
                0 :
                Errors.Count(w => w.ErrorLevel == Error.ErrorLevel.Warning);
        }
    }
}