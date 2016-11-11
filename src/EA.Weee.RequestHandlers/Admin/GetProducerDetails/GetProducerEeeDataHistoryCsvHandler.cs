namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    internal class GetProducerEeeDataHistoryCsvHandler : IRequestHandler<GetProducerEeeDataHistoryCsv, CSVFileData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;

        public GetProducerEeeDataHistoryCsvHandler(IWeeeAuthorization authorization, WeeeContext context,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.context = context;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<CSVFileData> HandleAsync(GetProducerEeeDataHistoryCsv request)
        {
            authorization.EnsureCanAccessInternalArea();
            if (string.IsNullOrEmpty(request.PRN))
            {
                throw new ArgumentException("PRN is required.");
            }

            string fileContent = await GetProducersEeeHistoryCsvContent(request.PRN);

            var fileName = string.Format("{0}_EEEdatachanges_{1:ddMMyyyy_HHmm}.csv",
                request.PRN,
                DateTime.UtcNow);

            return new CSVFileData
            {
                FileContent = fileContent,
                FileName = fileName
            };
        }

        private async Task<string> GetProducersEeeHistoryCsvContent(string prn)
        {
            var items = await context.StoredProcedures.SpgProducerEeeHistoryCsvData(prn);

            CsvWriter<EeeHistoryCsvResult> csvWriter =
                csvWriterFactory.Create<EeeHistoryCsvResult>();

            IEnumerable<EeeHistoryCsvResult> csvResults = CreateResults(items);

            csvWriter.DefineColumn(@"PRN", i => i.PRN);
            csvWriter.DefineColumn(@"PCS name", i => i.SchemeName);
            csvWriter.DefineColumn(@"PCS approval number", i => i.ApprovalNumber);
            csvWriter.DefineColumn(@"Compliance year", i => i.ComplianceYear);
            csvWriter.DefineColumn(@"Date and time (GMT) data submitted", i => i.SubmittedDate);
            csvWriter.DefineColumn(@"Quarter", i => i.Quarter);
            csvWriter.DefineColumn(@"Latest data", i => i.LatestData);
            foreach (int category in Enumerable.Range(1, 14))
            {
                    string title = string.Format("Cat{0} B2C", category);
                    string columnName = string.Format("Cat{0}B2C", category);
                    csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));               
            }
            foreach (int category in Enumerable.Range(1, 14))
            {
                string title = string.Format("Cat{0} B2B", category);
                string columnName = string.Format("Cat{0}B2B", category);
                csvWriter.DefineColumn(title, i => i.GetType().GetProperty(columnName).GetValue(i));
            }
            string fileContent = csvWriter.Write(csvResults);
            return fileContent;
        }

        public IEnumerable<EeeHistoryCsvResult> CreateResults(ProducerEeeHistoryCsvData results)
        {
            List<EeeHistoryCsvResult> csvResults = new List<EeeHistoryCsvResult>();          
          
            var uniqueSetsforHistory = results.ProducerReturnsHistoryData.GroupBy(p => new { p.ApprovalNumber, p.ComplianceYear, p.Quarter, p.SchemeName, p.PRN })
                   .Select(x => new { x.Key.ApprovalNumber, x.Key.ComplianceYear, x.Key.Quarter, x.Key.SchemeName, x.Key.PRN, count = x.Count() });

            var uniqueSetsforRemovedHistory = results.ProducerRemovedFromReturnsData.GroupBy(p => new { p.ApprovalNumber, p.ComplianceYear, p.Quarter})
                  .Select(x => new { x.Key.ApprovalNumber, x.Key.ComplianceYear, x.Key.Quarter, count = x.Count() });

            foreach (var set in uniqueSetsforHistory)
            {
                string prn = set.PRN;
                string schemeName = set.SchemeName;
                List<ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult> removedList = new List<ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult>();

                //for each (scheme, year and quarter) combination in ProducerEEEHisory resultset
                //get the earliest date, discards the results from removed producers result set which are less than earliest set
                var resultforHistoryDataSet = results.ProducerReturnsHistoryData.Where(p => (p.ApprovalNumber == set.ApprovalNumber 
                                                                                    && p.Quarter == set.Quarter 
                                                                                    && p.ComplianceYear == set.ComplianceYear));

                var resultforRemovedDataSet = results.ProducerRemovedFromReturnsData.Where(p => (p.ApprovalNumber == set.ApprovalNumber
                                                                                    && p.Quarter == set.Quarter
                                                                                    && p.ComplianceYear == set.ComplianceYear));

                var earliestDateForSet = resultforHistoryDataSet.Min(d => d.SubmittedDate);
                var maxDateForSet = resultforHistoryDataSet.Max(d => d.SubmittedDate);
                if (resultforRemovedDataSet.Any())
                {
                    //discard for all records from removedproducer result set where date is less than earliestDate for this set
                    var newRemovedResultSet = resultforRemovedDataSet.Where(r => (r.SubmittedDate > earliestDateForSet));
                    
                    //removed duplicate submissions which happened after the producer was removed
                    if (newRemovedResultSet.Any())
                    {
                        var newRemovedProducerResultSet = newRemovedResultSet.Where(p => p.SubmittedDate > maxDateForSet);
                        if (newRemovedProducerResultSet.Any())
                        {
                            removedList.Add(newRemovedProducerResultSet.First());
                            removedList.AddRange(newRemovedResultSet.Where(d => d.SubmittedDate < newRemovedProducerResultSet.First().SubmittedDate));
                        }
                        else
                        {
                            removedList.AddRange(newRemovedResultSet);
                        }
                    }
                }
                List<ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult> duplicateItemsToRemove = new List<ProducerEeeHistoryCsvData.ProducerRemovedFromReturnsResult>();
                for (int i = 0; i < resultforHistoryDataSet.Count(); i++)
                {                    
                    var item = resultforHistoryDataSet.ElementAt(i);
                    var nextItem = resultforHistoryDataSet.ElementAtOrDefault(i + 1);

                    //remove duplicate submission between 2 submission in case where producer was removed and and then added back after few submissions.
                    if (removedList.Any())
                    {
                        if (removedList.Count > 1 && item != null && nextItem != null)
                        {
                            var duplicates = removedList.Where(p => p.SubmittedDate > item.SubmittedDate && p.SubmittedDate < nextItem.SubmittedDate).Skip(1);
                            if (duplicates.Any())
                            {
                                foreach (var it in duplicates)
                                {
                                    duplicateItemsToRemove.Add(it);
                                }
                            }
                        }
                    }
                    EeeHistoryCsvResult row = new EeeHistoryCsvResult(item.PRN, item.ApprovalNumber, item.SchemeName, item.ComplianceYear,
                                                                     item.Quarter, item.SubmittedDate, item.LatestData,
                                                                     item.Cat1B2C, item.Cat2B2C, item.Cat3B2C, item.Cat4B2C,
                                                                     item.Cat5B2C, item.Cat6B2C, item.Cat7B2C, item.Cat8B2C,
                                                                     item.Cat9B2C, item.Cat10B2C, item.Cat11B2C, item.Cat12B2C,
                                                                     item.Cat13B2C, item.Cat14B2C,
                                                                     item.Cat1B2B, item.Cat2B2B, item.Cat3B2B, item.Cat4B2B,
                                                                     item.Cat5B2B, item.Cat6B2B, item.Cat7B2B, item.Cat8B2B,
                                                                     item.Cat9B2B, item.Cat10B2B, item.Cat11B2B, item.Cat12B2B,
                                                                     item.Cat13B2B, item.Cat14B2B);
                    csvResults.Add(row);
                }
                //remove consecutive items from removed list 
                foreach (var duplicateItem in duplicateItemsToRemove)
                {
                    removedList.Remove(duplicateItem);
                }
                //add all the records for the returns in which producer was removed.
                foreach (var item in removedList)
                {
                    string latestData = (item.SubmittedDate > maxDateForSet) ? "Yes" : "No";
                    if (latestData == "Yes")
                    {
                        //Set "no" for the max date data from producer history result set
                        csvResults.Single(s => s.SubmittedDate == maxDateForSet).LatestData = "No";
                    }
                    
                    EeeHistoryCsvResult row = new EeeHistoryCsvResult(prn, item.ApprovalNumber, schemeName, item.ComplianceYear,
                                                                     item.Quarter, item.SubmittedDate, latestData);
                    csvResults.Add(row);
                }
            }
           
            return csvResults.OrderByDescending(r => r.ComplianceYear).ThenByDescending(r => r.SubmittedDate);
        }

        public class EeeHistoryCsvResult
        {
            public string PRN { get; set; }
            public string SchemeName { get; set; }
            public string ApprovalNumber { get; set; }
            public int ComplianceYear { get; set; }
            public DateTime SubmittedDate { get; set; }
            public int Quarter { get; set; }
            public string LatestData { get; set; }
            public decimal? Cat1B2C { get; set; }
            public decimal? Cat2B2C { get; set; }
            public decimal? Cat3B2C { get; set; }
            public decimal? Cat4B2C { get; set; }
            public decimal? Cat5B2C { get; set; }
            public decimal? Cat6B2C { get; set; }
            public decimal? Cat7B2C { get; set; }
            public decimal? Cat8B2C { get; set; }
            public decimal? Cat9B2C { get; set; }
            public decimal? Cat10B2C { get; set; }
            public decimal? Cat11B2C { get; set; }
            public decimal? Cat12B2C { get; set; }
            public decimal? Cat13B2C { get; set; }
            public decimal? Cat14B2C { get; set; }
            public decimal? Cat1B2B { get; set; }
            public decimal? Cat2B2B { get; set; }
            public decimal? Cat3B2B { get; set; }
            public decimal? Cat4B2B { get; set; }
            public decimal? Cat5B2B { get; set; }
            public decimal? Cat6B2B { get; set; }
            public decimal? Cat7B2B { get; set; }
            public decimal? Cat8B2B { get; set; }
            public decimal? Cat9B2B { get; set; }
            public decimal? Cat10B2B { get; set; }
            public decimal? Cat11B2B { get; set; }
            public decimal? Cat12B2B { get; set; }
            public decimal? Cat13B2B { get; set; }
            public decimal? Cat14B2B { get; set; }

            public EeeHistoryCsvResult(string prn, string approvalNumber, string schemeName, int year, int quarter, DateTime date, string latestData,
                decimal? cat1b2c = null, decimal? cat2b2c = null, decimal? cat3b2c = null, decimal? cat4b2c = null, decimal? cat5b2c = null,
                decimal? cat6b2c = null, decimal? cat7b2c = null, decimal? cat8b2c = null, decimal? cat9b2c = null, decimal? cat10b2c = null,
                decimal? cat11b2c = null, decimal? cat12b2c = null, decimal? cat13b2c = null, decimal? cat14b2c = null,
                decimal? cat1b2b = null, decimal? cat2b2b = null, decimal? cat3b2b = null, decimal? cat4b2b = null, decimal? cat5b2b = null,
                decimal? cat6b2b = null, decimal? cat7b2b = null, decimal? cat8b2b = null, decimal? cat9b2b = null, decimal? cat10b2b = null,
                decimal? cat11b2b = null, decimal? cat12b2b = null, decimal? cat13b2b = null, decimal? cat14b2b = null)
            {
                PRN = prn;
                ApprovalNumber = approvalNumber;
                SchemeName = schemeName;
                ComplianceYear = year;
                Quarter = quarter;
                SubmittedDate = date;
                LatestData = latestData;
                Cat1B2C = cat1b2c;
                Cat2B2C = cat2b2c;
                Cat3B2C = cat3b2c;
                Cat4B2C = cat4b2c;
                Cat5B2C = cat5b2c;
                Cat6B2C = cat6b2c;
                Cat7B2C = cat7b2c;
                Cat8B2C = cat8b2c;
                Cat9B2C = cat9b2c;
                Cat10B2C = cat10b2c;
                Cat11B2C = cat11b2c;
                Cat12B2C = cat12b2c;
                Cat13B2C = cat13b2c;
                Cat14B2C = cat14b2c;
                //B2B
                Cat1B2B = cat1b2b;
                Cat2B2B = cat2b2b;
                Cat3B2B = cat3b2b;
                Cat4B2B = cat4b2b;
                Cat5B2B = cat5b2b;
                Cat6B2B = cat6b2b;
                Cat7B2B = cat7b2b;
                Cat8B2B = cat8b2b;
                Cat9B2B = cat9b2b;
                Cat10B2B = cat10b2b;
                Cat11B2B = cat11b2b;
                Cat12B2B = cat12b2b;
                Cat13B2B = cat13b2b;
                Cat14B2B = cat14b2b;
            }        
        }
    }
}
