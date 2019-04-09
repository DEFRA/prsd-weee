namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.Producer;
    using Domain.Producer.Classification;
    using Domain.Scheme;

    public class MigrationRegisteredProducerDataAccess : IMigrationRegisteredProducerDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationRegisteredProducerDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public void Add(RegisteredProducer registeredProducer)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisteredProducer> GetProducerRegistration(Guid id)
        {
            var producer = await context.RegisteredProducers.SingleOrDefaultAsync(p => p.Id == id);

            if (producer == null)
            {
                throw new ArgumentException(string.Format("Producer with Id '{0}' does not exist", id));
            }

            return producer;
        }

        public async Task<IEnumerable<RegisteredProducer>> GetProducerRegistrations(string producerRegistrationNumber, int complianceYear)
        {
            return await context.RegisteredProducers
                .Where(p => p.ProducerRegistrationNumber == producerRegistrationNumber
                    && p.ComplianceYear == complianceYear)
                .ToListAsync();
        }

        public ProducerSubmission GetProducerRegistration(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, MemberUpload memberUpload)
        {
            var producerSubmissions = context.ProducerSubmissions
                .Where(p => p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                            && p.RegisteredProducer.ComplianceYear == complianceYear
                            && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                            && p.MemberUpload.IsSubmitted
                            && p.MemberUpload.CreatedDate < memberUpload.CreatedDate
                            && p.RegisteredProducer.CurrentSubmission != null).ToList();

            var producerSubmissionsByDate = producerSubmissions.OrderByDescending(p => p.UpdatedDate).ToList();           

            if (producerSubmissionsByDate.Any())
            {
                return producerSubmissionsByDate.ElementAt(0);
            }

            return null;
        }

        public ProducerSubmission GetProducerRegistrationForInsert(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, MemberUpload upload, string name)
        {
            var producer = context.ProducerSubmissions.Where(p => p.ProducerBusiness.CompanyDetails != null && p.ProducerBusiness.CompanyDetails.Name.Equals(name)
                                                        || (p.ProducerBusiness.Partnership != null && p.ProducerBusiness.Partnership.Name.Equals(name))).ToList();

            var producerv2 = producer.Where(p => p.UpdatedDate < upload.CreatedDate && p.MemberUploadId != upload.Id).ToList();

            var registeredProducer = producerv2.FirstOrDefault(c => c.RegisteredProducer.ComplianceYear == complianceYear && c.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                                                                      && c.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber);

            return registeredProducer;
        }

        public bool HasPreviousAmendmentCharge(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, MemberUpload memberUpload)
        {
            var insert = context.ProducerSubmissions.Where(p => p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                                                                && p.RegisteredProducer.ComplianceYear == complianceYear
                                                                && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                                                                && p.MemberUpload.IsSubmitted
                                                                && p.MemberUpload.CreatedDate < memberUpload.CreatedDate
                                                                && p.MemberUpload.Id != memberUpload.Id //check
                                                                && (p.StatusType.HasValue && p.StatusType == StatusType.Insert.Value)).AsNoTracking().FirstOrDefault();

            if (insert != null)
            {
                return SubmissionsAfterDate(producerRegistrationNumber, complianceYear, schemeApprovalNumber, StatusType.Amendment, insert.UpdatedDate);
            }

            // insert can be null as amendment can be first charge in the year
            var initialAmendment = context.ProducerSubmissions.Where(p =>
                p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                && p.RegisteredProducer.ComplianceYear == complianceYear
                && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                && p.MemberUpload.IsSubmitted
                && p.MemberUpload.CreatedDate < memberUpload.CreatedDate
                && p.MemberUpload.Id != memberUpload.Id //check
                && (p.StatusType.HasValue && p.StatusType == StatusType.Amendment.Value)).AsNoTracking().OrderBy(p => p.UpdatedDate).FirstOrDefault();

            if (initialAmendment != null)
            {
                return SubmissionsAfterDate(producerRegistrationNumber, complianceYear, schemeApprovalNumber, StatusType.Amendment, initialAmendment.UpdatedDate);
            }

            return false;
        }

        private bool SubmissionsAfterDate(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, StatusType status,
            DateTime date)
        {
            var query = context.ProducerSubmissions.Where(p =>
                p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                && p.RegisteredProducer.ComplianceYear == complianceYear
                && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                && (p.StatusType.HasValue && p.StatusType == StatusType.Amendment.Value)
                && p.MemberUpload.IsSubmitted
                && p.ChargeThisUpdate > 0).ToList();

            var result = query.Any(c => c.UpdatedDate > date);

            return result;
        }
    }
}
