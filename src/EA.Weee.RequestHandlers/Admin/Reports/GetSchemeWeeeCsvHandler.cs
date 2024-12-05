﻿namespace EA.Weee.RequestHandlers.Admin.Reports
{
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using Domain.DataReturns;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Admin.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Admin.Reports;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetSchemeWeeeCsvHandler : IRequestHandler<GetSchemeWeeeCsv, FileInfo>
    {
        private readonly IStoredProcedures storedProcedures;
        private readonly IWeeeAuthorization authorization;
        private readonly CsvWriterFactory csvWriterFactory;

        private readonly Dictionary<int, string> categories = ReportHelper.GetCategoryDisplayNames();

        public GetSchemeWeeeCsvHandler(
            IStoredProcedures storedProcedures,
            IWeeeAuthorization authorization,
            CsvWriterFactory csvWriterFactory)
        {
            this.storedProcedures = storedProcedures;
            this.authorization = authorization;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<FileInfo> HandleAsync(GetSchemeWeeeCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            string obligationType = ConvertEnumToDatabaseString(message.ObligationType);

            var results = await storedProcedures.SpgSchemeWeeeCsvAsync(message.ComplianceYear, message.SchemeId, obligationType);

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

            string fileName;

            if (message.SchemeId != null)
            {
                SpgSchemeWeeeCsvResult.SchemeResult scheme = results.Schemes.Single();

                fileName = string.Format(
                    "{0}_{1}_{2}_schemeWEEE_{3:ddMMyyyy_HHmm}.csv",
                    message.ComplianceYear,
                    scheme.ApprovalNumber.Replace("/", string.Empty),
                    message.ObligationType,
                    SystemTime.UtcNow);
            }
            else
            {
                fileName = string.Format(
                    "{0}_{1}_schemeWEEE_{2:ddMMyyyy_HHmm}.csv",
                    message.ComplianceYear,
                    message.ObligationType,
                    SystemTime.UtcNow);
            }

            return new FileInfo(fileName, data);
        }

        public IEnumerable<CsvResult> CreateResults(SpgSchemeWeeeCsvResult results, IEnumerable<string> aatfLocations, IEnumerable<string> aaeLocations)
        {
            var csvResults = new List<CsvResult>();

            var collectedAmountsDictionary = new Dictionary<WeeeAmountKey, IEnumerable<SpgSchemeWeeeCsvResult.CollectedAmountResult>>();
            var deliveredAmountsDictionary = new Dictionary<WeeeAmountKey, IEnumerable<SpgSchemeWeeeCsvResult.DeliveredAmountResult>>();

            foreach (var quarterType in Enumerable.Range(1, 4))
            {
                foreach (var category in Enumerable.Range(1, 14))
                {
                    foreach (var scheme in results.Schemes)
                    {
                        var key = new WeeeAmountKey(category, quarterType, scheme.SchemeId);

                        collectedAmountsDictionary.Add(key, results.CollectedAmounts
                            .Where(ca => ca.QuarterType == key.QuarterType
                            && ca.WeeeCategory == key.Category
                            && ca.SchemeId == key.SchemeId));

                        deliveredAmountsDictionary.Add(key, results.DeliveredAmounts
                            .Where(ca => ca.QuarterType == key.QuarterType
                            && ca.WeeeCategory == key.Category
                            && ca.SchemeId == key.SchemeId));
                    }
                }
            }

            foreach (var key in collectedAmountsDictionary.Keys)
            {
                var collectedAmounts = collectedAmountsDictionary[key].ToList();
                var deliveredAmounts = deliveredAmountsDictionary[key].ToList();
                var scheme = results.Schemes.Single(s => s.SchemeId == key.SchemeId);

                var aatfTonnage = aatfLocations
                    .Select(l => new
                    {
                        Location = l,
                        Tonnage = deliveredAmounts
                            .Where(da => da.LocationType == 0)
                            .Where(da => da.LocationApprovalNumber == l)
                            .Select(da => (decimal?)da.Tonnage)
                            .SingleOrDefault()
                    })
                    .ToDictionary(a => a.Location, a => a.Tonnage);

                var aaeTonnage = aaeLocations
                    .Select(l => new
                    {
                        Location = l,
                        Tonnage = deliveredAmounts
                            .Where(da => da.LocationType == 1)
                            .Where(da => da.LocationApprovalNumber == l)
                            .Select(da => (decimal?)da.Tonnage)
                            .SingleOrDefault()
                    })
                    .ToDictionary(a => a.Location, a => a.Tonnage);

                var dcf = collectedAmounts
                    .Where(ca => ca.SourceType == (int)WeeeCollectedAmountSourceType.Dcf)
                    .Select(ca => (decimal?)ca.Tonnage)
                    .SingleOrDefault();

                var distributors = collectedAmounts
                    .Where(ca => ca.SourceType == (int)WeeeCollectedAmountSourceType.Distributor)
                    .Select(ca => (decimal?)ca.Tonnage)
                    .SingleOrDefault();

                var finalHolders = collectedAmounts
                    .Where(ca => ca.SourceType == (int)WeeeCollectedAmountSourceType.FinalHolder)
                    .Select(ca => (decimal?)ca.Tonnage)
                    .SingleOrDefault();

                csvResults.Add(new CsvResult
                {
                    AatfTonnage = aatfTonnage,
                    AeTonnage = aaeTonnage,
                    QuarterType = key.QuarterType,
                    Category = key.Category,
                    Dcf = dcf,
                    Distributors = distributors,
                    FinalHolders = finalHolders,
                    SchemeName = scheme.SchemeName,
                    SchemeApprovalNumber = scheme.ApprovalNumber,
                    TotalDelivered = deliveredAmounts.Any()
                        ? deliveredAmounts.Sum(da => da.Tonnage)
                        : (decimal?)null
                });
            }

            return csvResults.OrderBy(r => r.SchemeName);
        }

        public CsvWriter<CsvResult> CreateWriter(ObligationType obligationType, IEnumerable<string> aatfLocations, IEnumerable<string> aaeLocations)
        {
            CsvWriter<CsvResult> writer = csvWriterFactory.Create<CsvResult>();

            writer.DefineColumn("Scheme name", x => x.SchemeName);
            writer.DefineColumn("Scheme approval No", x => x.SchemeApprovalNumber);
            writer.DefineColumn("Quarter", x => string.Format("Q{0}", x.QuarterType));
            writer.DefineColumn("Category", x => categories[x.Category]);
            writer.DefineColumn("DCF (t)", x => x.Dcf);

            if (obligationType == ObligationType.B2C)
            {
                writer.DefineColumn("Distributors (t)", x => x.Distributors);
                writer.DefineColumn("Final holders (t)", x => x.FinalHolders);
            }

            writer.DefineColumn("Total AATF/AE (t)", x => x.TotalDelivered);

            foreach (string aatfLocation in aatfLocations)
            {
                var columnTitle = string.Format("{0} (t)", aatfLocation);
                writer.DefineColumn(columnTitle, x => x.AatfTonnage[aatfLocation]);
            }

            foreach (string aaeLocation in aaeLocations)
            {
                var columnTitle = string.Format("{0} (t)", aaeLocation);
                writer.DefineColumn(columnTitle, x => x.AeTonnage[aaeLocation]);
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
            public decimal? Distributors { get; set; }
            public decimal? FinalHolders { get; set; }
            public decimal? TotalDelivered { get; set; }
            public Dictionary<string, decimal?> AatfTonnage { get; set; }
            public Dictionary<string, decimal?> AeTonnage { get; set; }

            public CsvResult()
            {
                AatfTonnage = new Dictionary<string, decimal?>();
                AeTonnage = new Dictionary<string, decimal?>();
            }
        }

        public class WeeeAmountKey : IEquatable<WeeeAmountKey>
        {
            private readonly int hashBase;

            private readonly int category;
            private readonly int quarterType;
            private readonly Guid schemeId;

            public int Category => category;

            public int QuarterType => quarterType;

            public Guid SchemeId => schemeId;

            public WeeeAmountKey(int category, int quarterType, Guid schemeId)
            {
                Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);

                this.category = category;
                this.quarterType = quarterType;
                this.schemeId = schemeId;

                hashBase = 13;
            }

            public bool Equals(WeeeAmountKey other)
            {
                if (other == null)
                {
                    return false;
                }

                return QuarterType == other.QuarterType
                       && Category == other.Category
                       && SchemeId == other.SchemeId;
            }

            public override int GetHashCode()
            {
                var hash = hashBase;

                hash = (hashBase * 7) + Category;
                hash = (hash * 7) + QuarterType;
                hash = (hash * 7) + SchemeId.GetHashCode();

                return hash;
            }
        }
    }
}
