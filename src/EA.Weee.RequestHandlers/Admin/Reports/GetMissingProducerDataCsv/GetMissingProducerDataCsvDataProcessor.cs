namespace EA.Weee.RequestHandlers.Admin.Reports.GetMissingProducerDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    
    public class GetMissingProducerDataCsvDataProcessor : IGetMissingProducerDataCsvDataProcessor
    {
        private readonly WeeeContext context;

        public GetMissingProducerDataCsvDataProcessor(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<MissingProducerDataCsvData>> FetchMissingProducerDataAsync(int complianceYear,
            string obligationType,
            int? quarter,
            Guid? schemeId)
        {
            // Fetch a list of all the producer data and associated quarters that HAVE been submitted
            List<MissingProducerDataCsvData> producerData = await context.StoredProcedures.SpgMissingProducerDataCsvData(
                complianceYear, obligationType, quarter, schemeId);

            List<MissingProducerDataCsvData> missingData = new List<MissingProducerDataCsvData>();

            if (quarter.HasValue)
            {
                missingData = GetMissingQuarterData(producerData, quarter.Value);
            }
            else
            {
                foreach (int q in Enumerable.Range(1, 4))
                {
                    // Construct the union of the missing data for each quarter.
                    missingData.AddRange(GetMissingQuarterData(producerData, q));
                }
            }
            return missingData.OrderBy(d => d.SchemeName).ThenBy(d => d.ProducerName).ThenBy(d => d.Quarter).ToList();
        }

        private List<MissingProducerDataCsvData> GetMissingQuarterData(List<MissingProducerDataCsvData> dbData,
            int quarter)
        {
            // Get a list of PRNs that have submitted data for the specified quarter
            List<string> producersWithDataForQuarter = dbData
                .Where(p => p.Quarter.HasValue && p.Quarter.Value == quarter)
                .Select(p => p.PRN)
                .ToList();
            // Extract the data for producers that have not submitted for that quarter. As there are multiple entries, select one of
            // each producer/scheme pair (all other data is unique to that pair except quarter which will be updated)
            List<MissingProducerDataCsvData> missingProducers = dbData
                .Where(p => !producersWithDataForQuarter.Contains(p.PRN))
                .GroupBy(p => new { p.PRN, p.ApprovalNumber })
                .Select(p => p.First())
                .ToList();

            List<MissingProducerDataCsvData> missingData = new List<MissingProducerDataCsvData>();

            foreach (MissingProducerDataCsvData mp in missingProducers)
            {
                // Set all quarter data for missing producers to the missing one
                MissingProducerDataCsvData missingCopy = mp.Copy();
                missingCopy.Quarter = quarter;
                missingData.Add(missingCopy);
            }

            return missingData;
        }
    }
}