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

        public async Task<bool> HasPreviousAmendmentCharge(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber)
        {
            var insert = await context.ProducerSubmissions.Where(p => p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                                                                      && p.RegisteredProducer.ComplianceYear == complianceYear
                                                                      && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                                                                      && (p.StatusType.HasValue && p.StatusType == StatusType.Insert.Value)).FirstOrDefaultAsync();

            if (insert != null)
            {
                return await SubmissionsAfterDate(producerRegistrationNumber, complianceYear, schemeApprovalNumber, StatusType.Amendment, insert.UpdatedDate).AnyAsync();
            }
        
            // insert can be null as amendment can be first charge in the year
            var initialAmendment = await context.ProducerSubmissions.Where(p =>
                p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                && p.RegisteredProducer.ComplianceYear == complianceYear
                && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                && (p.StatusType.HasValue && p.StatusType == StatusType.Amendment.Value)).OrderBy(p => p.UpdatedDate).FirstOrDefaultAsync();

            if (initialAmendment != null)
            {
                return await SubmissionsAfterDate(producerRegistrationNumber, complianceYear, schemeApprovalNumber, StatusType.Amendment, initialAmendment.UpdatedDate).AnyAsync();
            }

            return false;
        }

        private IQueryable<ProducerSubmission> SubmissionsAfterDate(string producerRegistrationNumber, int complianceYear, string schemeApprovalNumber, StatusType status,
            DateTime date)
        {
            return context.ProducerSubmissions.Where(p =>
                p.RegisteredProducer.ProducerRegistrationNumber == producerRegistrationNumber
                && p.RegisteredProducer.ComplianceYear == complianceYear
                && p.RegisteredProducer.Scheme.ApprovalNumber == schemeApprovalNumber
                && (p.StatusType.HasValue && p.StatusType == StatusType.Amendment.Value)
                && p.UpdatedDate > date
                && p.ChargeThisUpdate > 0).AsQueryable();
        }
    }
}
