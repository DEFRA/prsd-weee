namespace EA.Weee.DataAccess.DataAccess
{
    using EA.Weee.Domain.Producer;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetDirectProducerSubmissionActiveComplianceYearsDataAccess : IGetDirectProducerSubmissionActiveComplianceYearsDataAccess
    {
        private readonly WeeeContext context;

        public GetDirectProducerSubmissionActiveComplianceYearsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<int>> Get()
        {
            return await context.DirectProducerSubmissions
                .Where(dru => dru.CurrentSubmission != null)
                .Where(dru => dru.DirectProducerSubmissionStatus.Value == DirectProducerSubmissionStatus.Complete.Value)
                .Select(dru => (int)dru.ComplianceYear)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync();
        }
    }
}
