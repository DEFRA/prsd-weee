namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Producer;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

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
                d.DirectRegistrantId == directRegistrantId && d.ComplianceYear == complianceYear && !d.RegisteredProducer.Removed)
                .FirstOrDefaultAsync();
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

        public async Task<DirectRegistrant> GetById(Guid directRegistrantId)
        {
            var directRegistrant = await context.DirectRegistrants
                .FirstOrDefaultAsync(d => d.Id == directRegistrantId);

            if (directRegistrant != null)
            {
                await context.Entry(directRegistrant)
                    .Collection(d => d.DirectProducerSubmissions)
                    .LoadAsync();

                foreach (var submission in directRegistrant.DirectProducerSubmissions.ToList())
                {
                    await context.Entry(submission)
                        .Reference(s => s.RegisteredProducer)
                        .LoadAsync();
                }

                directRegistrant.DirectProducerSubmissions = directRegistrant.DirectProducerSubmissions
                    .Where(dp => dp.RegisteredProducer != null && !dp.RegisteredProducer.Removed)
                    .ToList();
            }

            return directRegistrant;
        }
    }
}
