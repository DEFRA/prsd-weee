namespace EA.Weee.RequestHandlers.Admin.Reports.GetUKWeeeCsv
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.DataReturns;

    public class GetUkWeeeCsvDataAccess : IGetUkWeeeCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetUkWeeeCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns all data returns for the specified compliance year.
        /// Each data return will be pre-loaded with the current version's
        /// set of collected amounts and delivered amounts.
        /// The results will not be ordered.
        /// </summary>
        /// <param name="complianceYear"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DataReturn>> FetchDataReturnsForComplianceYearAsync(int complianceYear)
        {
            return await context.DataReturns
                .Include(dr => dr.CurrentVersion)
                .Include(dr => dr.CurrentVersion.WeeeCollectedReturnVersion)
                .Include(dr => dr.CurrentVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts)
                .Include(dr => dr.CurrentVersion.WeeeDeliveredReturnVersion)
                .Include(dr => dr.CurrentVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts)
                .Where(dr => dr.Quarter.Year == complianceYear)
                .ToListAsync();
        }
    }
}
