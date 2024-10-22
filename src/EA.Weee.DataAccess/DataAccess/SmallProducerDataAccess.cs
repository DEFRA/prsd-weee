namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class SmallProducerDataAccess : ISmallProducerDataAccess
    {
        private readonly WeeeContext context;

        public SmallProducerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionByComplianceYear(Guid directRegistrantId, int complianceYear)
        {
            return await context.DirectProducerSubmissions.Where(d =>
                d.DirectRegistrantId == directRegistrantId && d.ComplianceYear == complianceYear).FirstOrDefaultAsync();
        }

        public async Task<DirectProducerSubmission> GetCurrentDirectRegistrantSubmissionById(Guid directProducerSubmissionId)
        {
            return await context.DirectProducerSubmissions.Where(d =>
                d.Id == directProducerSubmissionId).FirstOrDefaultAsync();
        }

        public async Task<DirectRegistrant> GetDirectRegistrantByOrganisationId(Guid organisationId)
        {
            return await context.DirectRegistrants.Include(directRegistrant1 => directRegistrant1.Organisation)
                    .Include(directRegistrant2 => directRegistrant2.BrandName)
                    .Include(directRegistrant3 => directRegistrant3.Contact)
                    .Include(directRegistrant4 => directRegistrant4.Address)
                    .Include(directRegistrant5 => directRegistrant5.AdditionalCompanyDetails).FirstOrDefaultAsync(d =>
                        d.OrganisationId == organisationId);
        }
    }
}
