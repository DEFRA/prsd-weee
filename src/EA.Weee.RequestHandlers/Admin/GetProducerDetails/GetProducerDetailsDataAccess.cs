namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Producer;

    public class GetProducerDetailsDataAccess : IGetProducerDetailsDataAccess
    {
        private readonly WeeeContext context;

        public GetProducerDetailsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Fetches all submitted producer registrations with the specified
        /// registration number. The results will not be deterministically
        /// ordered.
        /// 
        /// All producer entities will be returned with member uploads,
        /// producer business, company and partnership relationships pre-loaded.
        /// 
        /// The returned entities will not be tracked for changes.
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <returns></returns>
        public async Task<List<ProducerSubmission>> Fetch(string registrationNumber)
        {
            return await context.ProducerSubmissions
                .Where(p => p.MemberUpload.IsSubmitted)
                .Where(p => p.RegisteredProducer.ProducerRegistrationNumber == registrationNumber)
                .Include(p => p.MemberUpload)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Include(p => p.ProducerBusiness.Partnership)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
