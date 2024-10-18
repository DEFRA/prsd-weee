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
    }
}
