namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Lookup;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.DataReturns;
    using Prsd.Core;
    using Security;

    public class FetchSummaryCsvHandler : IRequestHandler<Requests.DataReturns.FetchSummaryCsv, FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IFetchSummaryCsvDataAccess dataAccess;

        private List<CsvRow> csvRows = new List<CsvRow>();

        public FetchSummaryCsvHandler(
            IWeeeAuthorization authorization,
            CsvWriterFactory csvWriterFactory,
            IFetchSummaryCsvDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.csvWriterFactory = csvWriterFactory;
            this.dataAccess = dataAccess;

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE collected from DCFs",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.WeeeCollectedReturnVersion ?? new WeeeCollectedReturnVersion()).WeeeCollectedAmounts
                    .Where(wca => wca.SourceType == WeeeCollectedAmountSourceType.Dcf)
                    .Where(wca => wca.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE collected from DCFs",
                ObligationType = ObligationType.B2B,
                Selector = drv => (drv.WeeeCollectedReturnVersion ?? new WeeeCollectedReturnVersion()).WeeeCollectedAmounts
                    .Where(wca => wca.SourceType == WeeeCollectedAmountSourceType.Dcf)
                    .Where(wca => wca.ObligationType == Domain.Obligation.ObligationType.B2B)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE from distributors",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.WeeeCollectedReturnVersion ?? new WeeeCollectedReturnVersion()).WeeeCollectedAmounts
                    .Where(wca => wca.SourceType == WeeeCollectedAmountSourceType.Distributor)
                    .Where(wca => wca.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE from final holders",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.WeeeCollectedReturnVersion ?? new WeeeCollectedReturnVersion()).WeeeCollectedAmounts
                    .Where(wca => wca.SourceType == WeeeCollectedAmountSourceType.FinalHolder)
                    .Where(wca => wca.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE sent to AATFs",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.WeeeDeliveredReturnVersion ?? new WeeeDeliveredReturnVersion()).WeeeDeliveredAmounts
                    .Where(wda => wda.IsAatfDeliveredAmount)
                    .Where(wda => wda.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE sent to AATFs",
                ObligationType = ObligationType.B2B,
                Selector = drv => (drv.WeeeDeliveredReturnVersion ?? new WeeeDeliveredReturnVersion()).WeeeDeliveredAmounts
                    .Where(wda => wda.IsAatfDeliveredAmount)
                    .Where(wda => wda.ObligationType == Domain.Obligation.ObligationType.B2B)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE sent to AEs",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.WeeeDeliveredReturnVersion ?? new WeeeDeliveredReturnVersion()).WeeeDeliveredAmounts
                    .Where(wda => wda.IsAeDeliveredAmount)
                    .Where(wda => wda.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "WEEE sent to AEs",
                ObligationType = ObligationType.B2B,
                Selector = drv => (drv.WeeeDeliveredReturnVersion ?? new WeeeDeliveredReturnVersion()).WeeeDeliveredAmounts
                    .Where(wda => wda.IsAeDeliveredAmount)
                    .Where(wda => wda.ObligationType == Domain.Obligation.ObligationType.B2B)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "EEE placed on market",
                ObligationType = ObligationType.B2C,
                Selector = drv => (drv.EeeOutputReturnVersion ?? new EeeOutputReturnVersion()).EeeOutputAmounts
                    .Where(eoa => eoa.ObligationType == Domain.Obligation.ObligationType.B2C)
            });

            csvRows.Add(new CsvRow()
            {
                Description = "EEE placed on market",
                ObligationType = ObligationType.B2B,
                Selector = drv => (drv.EeeOutputReturnVersion ?? new EeeOutputReturnVersion()).EeeOutputAmounts
                    .Where(eoa => eoa.ObligationType == Domain.Obligation.ObligationType.B2B)
            });
        }

        public async Task<FileInfo> HandleAsync(Requests.DataReturns.FetchSummaryCsv message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationId);

            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeAsync(message.OrganisationId);

            CsvWriter<CsvResult> csvWriter = CreateWriter();

            DataReturn dataReturnQ1 = await dataAccess.FetchDataReturnOrDefaultAsync(message.OrganisationId, message.ComplianceYear, QuarterType.Q1);
            DataReturn dataReturnQ2 = await dataAccess.FetchDataReturnOrDefaultAsync(message.OrganisationId, message.ComplianceYear, QuarterType.Q2);
            DataReturn dataReturnQ3 = await dataAccess.FetchDataReturnOrDefaultAsync(message.OrganisationId, message.ComplianceYear, QuarterType.Q3);
            DataReturn dataReturnQ4 = await dataAccess.FetchDataReturnOrDefaultAsync(message.OrganisationId, message.ComplianceYear, QuarterType.Q4);

            List<CsvResult> csvResults = new List<CsvResult>();

            if (dataReturnQ1 != null)
            {
                IEnumerable<CsvResult> results = CreateResults(dataReturnQ1);
                csvResults.AddRange(results);
            }

            if (dataReturnQ2 != null)
            {
                IEnumerable<CsvResult> results = CreateResults(dataReturnQ2);
                csvResults.AddRange(results);
            }

            if (dataReturnQ3 != null)
            {
                IEnumerable<CsvResult> results = CreateResults(dataReturnQ3);
                csvResults.AddRange(results);
            }

            if (dataReturnQ4 != null)
            {
                IEnumerable<CsvResult> results = CreateResults(dataReturnQ4);
                csvResults.AddRange(results);
            }

            string content = csvWriter.Write(csvResults);
            byte[] data = Encoding.UTF8.GetBytes(content);

            string fileName = string.Format("{0}_EEE_WEEE_data_{1}_{2:ddMMyyyy_HHmm}.csv",
                scheme.ApprovalNumber,
                message.ComplianceYear,
                SystemTime.Now);

            return new FileInfo(fileName, data);
        }

        private class CsvRow
        {
            public string Description { get; set; }

            public ObligationType ObligationType { get; set; }

            public Func<DataReturnVersion, IEnumerable<ReturnItem>> Selector { get; set; }
        }

        public IEnumerable<CsvResult> CreateResults(DataReturn dataReturn)
        {
            if (dataReturn.CurrentVersion == null)
            {
                yield break;
            }

            foreach (CsvRow csvRow in csvRows)
            {
                IEnumerable<ReturnItem> returnItems = csvRow.Selector(dataReturn.CurrentVersion);

                if (returnItems.Any())
                {
                    CsvResult result = new CsvResult();

                    result.Quarter = dataReturn.Quarter.Q;
                    result.Description = csvRow.Description;
                    result.ObligationType = csvRow.ObligationType;

                    foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
                    {
                        decimal? total = null;

                        List<ReturnItem> returnItemsInCategory = returnItems
                            .Where(wca => wca.WeeeCategory == category)
                            .ToList();

                        if (returnItemsInCategory.Any())
                        {
                            total = returnItemsInCategory.Sum(wca => wca.Tonnage);
                        }

                        result.Amounts[category] = total;
                    }

                    yield return result;
                }
            }
        }

        public CsvWriter<CsvResult> CreateWriter()
        {
            CsvWriter<CsvResult> writer = csvWriterFactory.Create<CsvResult>();

            writer.DefineColumn("Quarter", r => r.Quarter);
            writer.DefineColumn("EEE or WEEE in tonnes (t)", r => r.Description);
            writer.DefineColumn("Obligation type", r => r.ObligationType);

            for (int i = 1; i <= 14; ++i)
            {
                // Create a copy of i that can be boxed and used by the lambda expression.
                int j = i;

                writer.DefineColumn(
                    string.Format("Cat {0} (t)", j),
                    r => r.Amounts[(WeeeCategory)j]);
            }

            return writer;
        }

        public class CsvResult
        {
            public QuarterType Quarter { get; set; }

            public string Description { get; set; }

            public ObligationType ObligationType { get; set; }

            public Dictionary<WeeeCategory, decimal?> Amounts { get; set; }

            public CsvResult()
            {
                Amounts = new Dictionary<WeeeCategory, decimal?>();
            }
        }
    }
}
