namespace EA.Weee.RequestHandlers.DataReturns.FetchSummaryCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using Prsd.Core;
    using Security;

    public class FetchSummaryCsvHandler : IRequestHandler<Requests.DataReturns.FetchSummaryCsv, FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly IFetchSummaryCsvDataAccess dataAccess;

        public FetchSummaryCsvHandler(
            IWeeeAuthorization authorization,
            CsvWriterFactory csvWriterFactory,
            IFetchSummaryCsvDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.csvWriterFactory = csvWriterFactory;
            this.dataAccess = dataAccess;
        }

        public async Task<FileInfo> HandleAsync(Requests.DataReturns.FetchSummaryCsv message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationId);

            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeAsync(message.OrganisationId);

            List<DataReturnSummaryCsvData> results = await dataAccess.FetchResultsAsync(
                scheme.Id,
                message.ComplianceYear);

            CsvWriter<DataReturnSummaryCsvData> csvWriter = CreateWriter();

            string content = csvWriter.Write(results);
            byte[] data = Encoding.UTF8.GetBytes(content);

            string fileName = string.Format("{0}_EEE_WEEE_data_{1}_{2:ddMMyyyy_HHmm}.csv",
                scheme.ApprovalNumber,
                message.ComplianceYear,
                SystemTime.Now);

            return new FileInfo(fileName, data);
        }
        
        public CsvWriter<DataReturnSummaryCsvData> CreateWriter()
        {
            CsvWriter<DataReturnSummaryCsvData> writer = csvWriterFactory.Create<DataReturnSummaryCsvData>();

            writer.DefineColumn("Quarter", r => string.Format("Q{0}", r.Quarter));
            writer.DefineColumn("EEE or WEEE in tonnes (t)", r => descriptions[new Tuple<int, int?>(r.Type, r.Source)]);
            writer.DefineColumn("Obligation type", r => r.ObligationType);
            writer.DefineColumn("Cat 1 (t)", r => string.Format("{0:F3}", r.Category1));
            writer.DefineColumn("Cat 2 (t)", r => string.Format("{0:F3}", r.Category2));
            writer.DefineColumn("Cat 3 (t)", r => string.Format("{0:F3}", r.Category3));
            writer.DefineColumn("Cat 4 (t)", r => string.Format("{0:F3}", r.Category4));
            writer.DefineColumn("Cat 5 (t)", r => string.Format("{0:F3}", r.Category5));
            writer.DefineColumn("Cat 6 (t)", r => string.Format("{0:F3}", r.Category6));
            writer.DefineColumn("Cat 7 (t)", r => string.Format("{0:F3}", r.Category7));
            writer.DefineColumn("Cat 8 (t)", r => string.Format("{0:F3}", r.Category8));
            writer.DefineColumn("Cat 9 (t)", r => string.Format("{0:F3}", r.Category9));
            writer.DefineColumn("Cat 10 (t)", r => string.Format("{0:F3}", r.Category10));
            writer.DefineColumn("Cat 11 (t)", r => string.Format("{0:F3}", r.Category11));
            writer.DefineColumn("Cat 12 (t)", r => string.Format("{0:F3}", r.Category12));
            writer.DefineColumn("Cat 13 (t)", r => string.Format("{0:F3}", r.Category13));
            writer.DefineColumn("Cat 14 (t)", r => string.Format("{0:F3}", r.Category14));

            return writer;
        }

        private static readonly Dictionary<Tuple<int, int?>, string> descriptions = new Dictionary<Tuple<int, int?>, string>()
        {
            { new Tuple<int, int?>(0, 0), "WEEE collected from DCFs" },
            { new Tuple<int, int?>(0, 1), "WEEE from distributors" },
            { new Tuple<int, int?>(0, 2), "WEEE from final holders" },
            { new Tuple<int, int?>(1, 0), "WEEE sent to AATFs" },
            { new Tuple<int, int?>(1, 1), "WEEE sent to AEs" },
            { new Tuple<int, int?>(2, null), "EEE placed on market" }
        };
    }
}
