﻿namespace EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using Domain.Lookup;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.Admin.Helpers;
    using Prsd.Core;
    using Requests.Admin.AatfReports;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetUkWeeeAtAatfsCsvHandler : IRequestHandler<GetUkWeeeAtAatfsCsv, FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetUkWeeeAtAatfsCsvDataAccess dataAccess;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly Dictionary<int, string> categories = ReportHelper.GetCategoryDisplayNames();

        public GetUkWeeeAtAatfsCsvHandler(
            IWeeeAuthorization authorization,
            IGetUkWeeeAtAatfsCsvDataAccess dataAccess,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<FileInfo> HandleAsync(GetUkWeeeAtAatfsCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            var returns = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(message.ComplianceYear);

            var csvResults = CreateResults(returns, message.ComplianceYear.ToString());

            var csvWriter = CreateWriter();

            var content = csvWriter.Write(csvResults);

            var data = Encoding.UTF8.GetBytes(content);

            var fileName = $"{message.ComplianceYear}_UK WEEE received at AATFs_{SystemTime.UtcNow:ddMMyyyy_HHmm}.csv";

            return new FileInfo(fileName, data);
        }

        public CsvWriter<CsvResult> CreateWriter()
        {
            var csvWriter = csvWriterFactory.Create<CsvResult>();

            csvWriter.DefineColumn("Quarter", x => x.TimePeriod);
            csvWriter.DefineColumn("Category", x => categories[(int)x.Category]);
            csvWriter.DefineColumn("B2C received for treatment (t)", x => x.B2cForTreatment);
            csvWriter.DefineColumn("B2C for reuse (t)", x => x.B2cForReuse);
            csvWriter.DefineColumn("B2C sent to AATF/ATF (t)", x => x.B2cForAatf);
            csvWriter.DefineColumn("B2B received for treatment (t)", x => x.B2bForTreatment);
            csvWriter.DefineColumn("B2B for reuse (t)", x => x.B2bForReuse);
            csvWriter.DefineColumn("B2B sent to AATF/ATF (t)", x => x.B2bForAatf);

            return csvWriter;
        }

        public IEnumerable<CsvResult> CreateResults(IEnumerable<PartialAatfReturn> returns, string year)
        {
            foreach (QuarterType quarter in Enum.GetValues(typeof(QuarterType)))
            {
                var quarterReturns = returns.Where(p => p.Quarter.Q == quarter);
                foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
                {
                    GetTotals(quarterReturns.SelectMany(r => r.ObligatedWeeeReceivedData), category, out var b2cForTreatment, out var b2bForTreatment);
                    GetTotals(quarterReturns.SelectMany(r => r.ObligatedWeeeReusedData), category, out var b2cForReuse, out var b2bForReuse);
                    GetTotals(quarterReturns.SelectMany(r => r.ObligatedWeeeSentOnData), category, out var b2cForAatf, out var b2bForAatf);

                    yield return new CsvResult(
                        quarter.ToString(),
                        category,
                        b2cForTreatment,
                        b2cForReuse,
                        b2cForAatf,
                        b2bForTreatment,
                        b2bForReuse,
                        b2bForAatf);
                }
            }

            foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
            {
                GetTotals(returns.SelectMany(q => q.ObligatedWeeeReceivedData), category, out var b2cForTreatment, out var b2bForTreatment);
                GetTotals(returns.SelectMany(q => q.ObligatedWeeeReusedData), category, out var b2cForReuse, out var b2bForReuse);
                GetTotals(returns.SelectMany(q => q.ObligatedWeeeSentOnData), category, out var b2cForAatf, out var b2bForAatf);

                yield return new CsvResult(
                    year,
                    category,
                    b2cForTreatment,
                    b2cForReuse,
                    b2cForAatf,
                    b2bForTreatment,
                    b2bForReuse,
                    b2bForAatf);
            }
        }

        private void GetTotals(
            IEnumerable<WeeeObligatedData> data,
            WeeeCategory category,
            out decimal? b2c,
            out decimal? b2b)
        {
            b2c = data.Where(r => r.CategoryId == (int)category).Sum(r => r.B2C);
            b2b = data.Where(r => r.CategoryId == (int)category).Sum(r => r.B2B);
        }

        public class CsvResult
        {
            public string TimePeriod { get; private set; }
            public WeeeCategory Category { get; private set; }
            public decimal? B2cForTreatment { get; private set; }
            public decimal? B2cForReuse { get; private set; }
            public decimal? B2cForAatf { get; private set; }
            public decimal? B2bForTreatment { get; private set; }
            public decimal? B2bForReuse { get; private set; }
            public decimal? B2bForAatf { get; private set; }

            public CsvResult(
                string timePeriod,
                WeeeCategory category,
                decimal? b2cForTreatment,
                decimal? b2cForReuse,
                decimal? b2cForAatf,
                decimal? b2bForTreatment,
                decimal? b2bForReuse,
                decimal? b2bForAatf)
            {
                TimePeriod = timePeriod;
                Category = category;
                B2cForTreatment = b2cForTreatment;
                B2cForReuse = b2cForReuse;
                B2cForAatf = b2cForAatf;
                B2bForTreatment = b2bForTreatment;
                B2bForReuse = b2bForReuse;
                B2bForAatf = b2bForAatf;
            }
        }
    }
}
