namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
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

        public async Task<IEnumerable<ProducerEeeByQuarter>> EeeOutputBySchemeAndComplianceYear(string registrationNumber, int complianceYear, Guid schemeId)
        {
            var eeeOutputAmountsByQuarter = (await context.DataReturns
                .Where(dr => dr.CurrentVersion != null)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .Select(
                    dr =>
                        new
                        {
                            dr.Quarter,
                            EeeOutputAmountIds =
                                dr.CurrentVersion.EeeOutputReturnVersion.EeeOutputAmounts.Select(eee => eee.Id)
                        })
                .ToListAsync());

            var eeeOutputReturnVersionsForProducerAndScheme = (await context.EeeOutputAmounts
                .Where(eee => eee.RegisteredProducer.ProducerRegistrationNumber == registrationNumber)
                .Where(eee => eee.RegisteredProducer.Scheme.Id == schemeId)
                .ToListAsync());

            return eeeOutputAmountsByQuarter
                .Select(dr => new ProducerEeeByQuarter
                {
                    Quarter = dr.Quarter,
                    Eee = dr.EeeOutputAmountIds
                        .Join(eeeOutputReturnVersionsForProducerAndScheme, eoa => eoa, eorv => eorv.Id,
                            (id, amount) => amount)
                });
        }
    }
}
