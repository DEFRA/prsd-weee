namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;

    public class GetSchemeWeeeCsvHandler : IRequestHandler<GetSchemeWeeeCsv, FileInfo>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly CsvWriterFactory csvWriterFactory;

        private readonly Dictionary<int, string> categoryDisplayNames = new Dictionary<int, string>()
        {
            { 1, "1. Large Household Appliances" },
            { 2, "2. Small Household Appliances" },
            { 3, "3. IT and Telecomms Equipment" },
            { 4, "4. Consumer Equipment" },
            { 5, "5. Lighting Equipment" },
            { 6, "6. Electrical and Electronic Tools" },
            { 7, "7. Toys Leisure and Sports" },
            { 8, "8. Medical Devices" },
            { 9, "9. Monitoring and Control Instruments" },
            { 10, "10. Automatic Dispensers" },
            { 11, "11. Display Equipment" },
            { 12, "12. Cooling Appliances Containing Refrigerants" },
            { 13, "13. Gas Discharge Lamps and LED Light Sources" },
            { 14, "14. Photovoltaic Panels" },
        };

        public GetSchemeWeeeCsvHandler(
            WeeeContext context,
            IWeeeAuthorization authorization,
            CsvWriterFactory csvWriterFactory)
        {
            this.context = context;
            this.authorization = authorization;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<FileInfo> HandleAsync(GetSchemeWeeeCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            string obligationType = ConvertEnumToDatabaseString(message.ObligationType);

            var results = await context.StoredProcedures.SpgSchemeWeeeCsvAsync(message.ComplianceYear, obligationType);

            IEnumerable<string> aatfLocations = results.DeliveredAmounts
                .Where(da => da.LocationType == 0)
                .Select(da => da.LocationApprovalNumber)
                .Distinct()
                .OrderBy(x => x);

            IEnumerable<string> aaeLocations = results.DeliveredAmounts
                .Where(da => da.LocationType == 1)
                .Select(da => da.LocationApprovalNumber)
                .Distinct()
                .OrderBy(x => x);

            IEnumerable<CsvResult> csvResults = CreateResults(results, aatfLocations, aaeLocations);

            CsvWriter<CsvResult> writer = CreateWriter(message.ObligationType, aatfLocations, aaeLocations);

            string contents = writer.Write(csvResults);

            byte[] data = Encoding.UTF8.GetBytes(contents);

            string fileName = string.Format(
                "{0}_{1}_schemeWEEEE_{2:ddMMyyyy_HHmm}.csv",
                message.ComplianceYear,
                message.ObligationType,
                SystemTime.UtcNow);

            return new FileInfo(fileName, data);
        }

        public IEnumerable<CsvResult> CreateResults(SpgSchemeWeeeCsvResult results, IEnumerable<string> aatfLocations, IEnumerable<string> aaeLocations)
        {
            List<CsvResult> csvResults = new List<CsvResult>();

            foreach (int quarterType in Enumerable.Range(1, 4))
            {
                foreach (int category in Enumerable.Range(1, 14))
                {
                    foreach (var scheme in results.Schemes.OrderBy(s => s.SchemeName))
                    {
                        var collectedAmounts = results.CollectedAmounts
                            .Where(ca => ca.QuarterType == quarterType)
                            .Where(ca => ca.WeeeCategory == category)
                            .Where(ca => ca.SchemeId == scheme.SchemeId);

                        decimal? dcf = collectedAmounts
                            .Where(ca => ca.SourceType == 0)
                            .Select(ca => (decimal?)ca.Tonnage)
                            .SingleOrDefault();

                        // A source type of "Distributor" has ID 1 and is defined by regulation 43.
                        decimal? r43 = collectedAmounts
                            .Where(ca => ca.SourceType == 1)
                            .Select(ca => (decimal?)ca.Tonnage)
                            .SingleOrDefault();

                        // A source type of "Final Holder" has ID 2 and is defined by regulation 52.
                        decimal? r52 = collectedAmounts
                            .Where(ca => ca.SourceType == 2)
                            .Select(ca => (decimal?)ca.Tonnage)
                            .SingleOrDefault();

                        CsvResult csvResult = new CsvResult();

                        csvResult.SchemeName = scheme.SchemeName;
                        csvResult.SchemeApprovalNumber = scheme.ApprovalNumber;
                        csvResult.QuarterType = quarterType;
                        csvResult.Category = category;
                        csvResult.Dcf = dcf;
                        csvResult.R43 = r43;
                        csvResult.R52 = r52;

                        var deliveredAmounts = results.DeliveredAmounts
                            .Where(da => da.QuarterType == quarterType)
                            .Where(da => da.WeeeCategory == category)
                            .Where(da => da.SchemeId == scheme.SchemeId);

                        decimal? totalDelivered = null;

                        if (deliveredAmounts.Any())
                        {
                            totalDelivered = deliveredAmounts.Sum(da => da.Tonnage);
                        }

                        csvResult.TotalDelivered = totalDelivered;

                        foreach (string aatfLocation in aatfLocations)
                        {
                            decimal? aatfTonnage = deliveredAmounts
                                .Where(da => da.LocationType == 0)
                                .Where(da => da.LocationApprovalNumber == aatfLocation)
                                .Select(da => (decimal?)da.Tonnage)
                                .SingleOrDefault();

                            csvResult.AatfTonnage[aatfLocation] = aatfTonnage;
                        }

                        foreach (string aaeLocation in aaeLocations)
                        {
                            decimal? aaeTonnage = deliveredAmounts
                                .Where(da => da.LocationType == 1)
                                .Where(da => da.LocationApprovalNumber == aaeLocation)
                                .Select(da => (decimal?)da.Tonnage)
                                .SingleOrDefault();

                            csvResult.AeTonnage[aaeLocation] = aaeTonnage;
                        }

                        csvResults.Add(csvResult);
                    }
                }
            }

            return csvResults;
        }

        public CsvWriter<CsvResult> CreateWriter(ObligationType obligationType, IEnumerable<string> aatfLocations, IEnumerable<string> aaeLocations)
        {
            CsvWriter<CsvResult> writer = csvWriterFactory.Create<CsvResult>();

            writer.DefineColumn("Scheme Name", x => x.SchemeName);
            writer.DefineColumn("Scheme approval No", x => x.SchemeApprovalNumber);
            writer.DefineColumn("Quarter", x => string.Format("Q{0}", x.QuarterType));
            writer.DefineColumn("Category", x => categoryDisplayNames[x.Category]);
            writer.DefineColumn("DCF", x => x.Dcf);

            if (obligationType == ObligationType.B2C)
            {
                writer.DefineColumn("R43", x => x.R43);
                writer.DefineColumn("R52", x => x.R52);
            }

            writer.DefineColumn("Total AATF/AE", x => x.TotalDelivered);

            foreach (string aatfLocation in aatfLocations)
            {
                writer.DefineColumn(aatfLocation, x => x.AatfTonnage[aatfLocation]);
            }

            foreach (string aaeLocation in aaeLocations)
            {
                writer.DefineColumn(aaeLocation, x => x.AeTonnage[aaeLocation]);
            }

            return writer;
        }

        private static string ConvertEnumToDatabaseString(ObligationType obligationType)
        {
            switch (obligationType)
            {
                case ObligationType.B2B:
                    return "B2B";

                case ObligationType.B2C:
                    return "B2C";

                case ObligationType.Both:
                case ObligationType.None:
                default:
                    throw new NotSupportedException();
            }
        }

        public class CsvResult
        {
            public string SchemeName { get; set; }
            public string SchemeApprovalNumber { get; set; }
            public int QuarterType { get; set; }
            public int Category { get; set; }
            public decimal? Dcf { get; set; }
            public decimal? R43 { get; set; }
            public decimal? R52 { get; set; }
            public decimal? TotalDelivered { get; set; }
            public Dictionary<string, decimal?> AatfTonnage { get; set; }
            public Dictionary<string, decimal?> AeTonnage { get; set; }

            public CsvResult()
            {
                AatfTonnage = new Dictionary<string, decimal?>();
                AeTonnage = new Dictionary<string, decimal?>();
            }
        }
    }
}
