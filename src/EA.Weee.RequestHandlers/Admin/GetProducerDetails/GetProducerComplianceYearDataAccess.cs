namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetProducerComplianceYearDataAccess : IGetProducerComplianceYearDataAccess
    {
        private readonly WeeeContext context;

        public GetProducerComplianceYearDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task<List<int>> GetComplianceYears(string registrationNumber)
        {
            return context.RegisteredProducers
                   .Where(rp => rp.ProducerRegistrationNumber == registrationNumber)
                   .Where(rp => rp.CurrentSubmission != null)
                   .Select(rp => rp.ComplianceYear)
                   .Distinct()
                   .OrderByDescending(year => year)
                   .ToListAsync();
        }
    }
}
