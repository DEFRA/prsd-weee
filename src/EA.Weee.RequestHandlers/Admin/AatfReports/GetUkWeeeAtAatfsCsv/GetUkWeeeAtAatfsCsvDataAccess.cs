namespace EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;

    public class GetUkWeeeAtAatfsCsvDataAccess : IGetUkWeeeAtAatfsCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetUkWeeeAtAatfsCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns all data returns for the specified compliance year.
        /// Each data return will be pre-loaded with the current version's
        /// set of collected amounts and delivered amounts.
        /// The results will not be ordered.
        /// </summary>
        public async Task<IEnumerable<PartialAatfReturn>> FetchPartialAatfReturnsForComplianceYearAsync(int complianceYear)
        {
            var returns = await context.Returns
                .Where(r => r.Quarter.Year == complianceYear)
                .Where(r => r.SubmittedDate.HasValue)
                .GroupBy(r => r.Organisation.Id)
                .SelectMany(orgGroup =>
                    orgGroup.GroupBy(r => r.Quarter.Q)
                    .Select(quarterGroup => quarterGroup.OrderByDescending(r => r.SubmittedDate.Value)
                    .FirstOrDefault()))
                .ToListAsync();

            var partialReturns = new List<PartialAatfReturn>();

            foreach (var @return in returns)
            {
                var weeeReceived = await context.WeeeReceived.Where(w => w.ReturnId == @return.Id).SelectMany(w => w.WeeeReceivedAmounts)
                    .Select(a => new WeeeObligatedData
                    {
                        CategoryId = a.CategoryId,
                        B2B = a.NonHouseholdTonnage,
                        B2C = a.HouseholdTonnage
                    }).ToListAsync();

                var weeeReused = await context.WeeeReused.Where(w => w.ReturnId == @return.Id).SelectMany(w => w.WeeeReusedAmounts)
                    .Select(a => new WeeeObligatedData
                    {
                        CategoryId = a.CategoryId,
                        B2B = a.NonHouseholdTonnage,
                        B2C = a.HouseholdTonnage
                    }).ToListAsync();

                var weeeSentOn = await context.WeeeSentOn.Where(w => w.ReturnId == @return.Id).SelectMany(w => w.WeeeSentOnAmounts)
                    .Select(a => new WeeeObligatedData
                    {
                        CategoryId = a.CategoryId,
                        B2B = a.NonHouseholdTonnage,
                        B2C = a.HouseholdTonnage
                    }).ToListAsync();

                partialReturns.Add(new PartialAatfReturn
                {
                    Quarter = @return.Quarter,
                    ObligatedWeeeReceivedData = weeeReceived,
                    ObligatedWeeeReusedData = weeeReused,
                    ObligatedWeeeSentOnData = weeeSentOn
                });
            }

            return partialReturns;
        }
    }
}
