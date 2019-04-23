namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Producer;
    using Domain.Producer.Classification;

    public class RegisteredProducerDataAccess : IRegisteredProducerDataAccess
    {
        private readonly WeeeContext context;

        public RegisteredProducerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public void Add(RegisteredProducer registeredProducer)
        {
            context.AllRegisteredProducers.Add(registeredProducer);
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

        public async Task<RegisteredProducer> GetProducerRegistration(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber)
        {
            var result = await context.RegisteredProducers
                .Where(p => p.ProducerRegistrationNumber == producerRegistrationNumber
                            && p.ComplianceYear == complianceYear
                            && p.Scheme.ApprovalNumber == schemeApprovalNumber
                            && p.CurrentSubmission != null)
                .ToListAsync();

            if (result.Count() > 1)
            {
                throw new ArgumentException(string.Format("Producer with registration number '{0}' for compliance year '{1}' and scheme '{2}' has more than one record", producerRegistrationNumber, complianceYear, schemeApprovalNumber));
            }

            if (result.Any())
            {
                return result.ElementAt(0);
            }

            return null;
        }

        public bool HasPreviousAmendmentCharge(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber)
        {
            var insert = context.ProducerSubmissions.Where(p => p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                                                                      && p.RegisteredProducer.ComplianceYear == complianceYear
                                                                      && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                                                                      && p.MemberUpload.IsSubmitted
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
