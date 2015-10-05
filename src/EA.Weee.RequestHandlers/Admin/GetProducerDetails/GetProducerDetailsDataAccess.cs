namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Producer;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetProducerDetailsDataAccess : IGetProducerDetailsDataAccess
    {
        private readonly WeeeContext context;

        public GetProducerDetailsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<Producer>> Fetch(string registrationNumber)
        {
            return await context.Producers
                .Where(p => p.MemberUpload.IsSubmitted)
                .Where(p => p.RegistrationNumber == registrationNumber)
                .OrderBy(p => p.RegistrationNumber)
                .Include(p => p.MemberUpload)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Include(p => p.ProducerBusiness.Partnership)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
