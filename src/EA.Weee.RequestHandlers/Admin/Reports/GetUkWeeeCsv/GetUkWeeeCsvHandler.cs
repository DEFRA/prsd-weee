namespace EA.Weee.RequestHandlers.Admin.Reports.GetUKWeeeCsv
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
    using EA.Weee.Requests.Admin.Reports;
    using Prsd.Core;
    using Security;

    public class GetUkWeeeCsvHandler : IRequestHandler<Requests.Admin.Reports.GetUkWeeeCsv, FileInfo>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetUkWeeeCsvDataAccess dataAccess;
        private readonly CsvWriterFactory csvWriterFactory;

        private readonly Dictionary<WeeeCategory, string> categoryDisplayNames = new Dictionary<WeeeCategory, string>()
        {
            { WeeeCategory.LargeHouseholdAppliances, "1. Large Household Appliances" },
            { WeeeCategory.SmallHouseholdAppliances, "2. Small Household Appliances" },
            { WeeeCategory.ITAndTelecommsEquipment, "3. IT and Telecomms Equipment" },
            { WeeeCategory.ConsumerEquipment, "4. Consumer Equipment" },
            { WeeeCategory.LightingEquipment, "5. Lighting Equipment" },
            { WeeeCategory.ElectricalAndElectronicTools, "6. Electrical and Electronic Tools" },
            { WeeeCategory.ToysLeisureAndSports, "7. Toys Leisure and Sports" },
            { WeeeCategory.MedicalDevices, "8. Medical Devices" },
            { WeeeCategory.MonitoringAndControlInstruments, "9. Monitoring and Control Instruments" },
            { WeeeCategory.AutomaticDispensers, "10. Automatic Dispensers" },
            { WeeeCategory.DisplayEquipment, "11. Display Equipment" },
            { WeeeCategory.CoolingApplicancesContainingRefrigerants, "12. Cooling Appliances Containing Refrigerants" },
            { WeeeCategory.GasDischargeLampsAndLedLightSources, "13. Gas Discharge Lamps and LED Light Sources" },
            { WeeeCategory.PhotovoltaicPanels, "14. Photovoltaic Panels" },
        };

        public GetUkWeeeCsvHandler(
            IWeeeAuthorization authorization,
            IGetUkWeeeCsvDataAccess dataAccess,
            CsvWriterFactory csvWriterFactory)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.csvWriterFactory = csvWriterFactory;
        }

        public async Task<FileInfo> HandleAsync(GetUkWeeeCsv message)
        {
            authorization.EnsureCanAccessInternalArea();

            IEnumerable<DataReturn> dataReturns = await dataAccess.FetchDataReturnsForComplianceYearAsync(message.ComplianceYear);

            IEnumerable<CsvResult> csvReuslts = CreateResults(dataReturns);

            CsvWriter<CsvResult> csvWriter = CreateWriter();

            string content = csvWriter.Write(csvReuslts);

            byte[] data = Encoding.UTF8.GetBytes(content);

            string fileName = string.Format("{0}_UK_WEEE_{1:ddMMyyyy_HHmm}.csv",
                message.ComplianceYear,
                SystemTime.UtcNow);

            return new FileInfo(fileName, data);
        }

        public CsvWriter<CsvResult> CreateWriter()
        {
            CsvWriter<CsvResult> csvWriter = csvWriterFactory.Create<CsvResult>();

            csvWriter.DefineColumn("Category", x => categoryDisplayNames[x.Category]);
            csvWriter.DefineColumn("Obligation type", x => x.ObligationType);
            csvWriter.DefineColumn("Total WEEE from DCF (t)", x => x.DcfTotal);
            csvWriter.DefineColumn("Q1 WEEE from DCF (t)", x => x.DcfQ1);
            csvWriter.DefineColumn("Q2 WEEE from DCF (t)", x => x.DcfQ2);
            csvWriter.DefineColumn("Q3 WEEE from DCF (t)", x => x.DcfQ3);
            csvWriter.DefineColumn("Q4 WEEE from DCF (t)", x => x.DcfQ4);
            csvWriter.DefineColumn("Total WEEE from distributors (t)", x => x.DistributorTotal);
            csvWriter.DefineColumn("Q1 WEEE from distributors (t)", x => x.DistributorQ1);
            csvWriter.DefineColumn("Q2 WEEE from distributors (t)", x => x.DistributorQ2);
            csvWriter.DefineColumn("Q3 WEEE from distributors (t)", x => x.DistributorQ3);
            csvWriter.DefineColumn("Q4 WEEE from distributors (t)", x => x.DistributorQ4);
            csvWriter.DefineColumn("Total WEEE from final holders (t)", x => x.FinalHolderTotal);
            csvWriter.DefineColumn("Q1 WEEE from final holders (t)", x => x.FinalHolderQ1);
            csvWriter.DefineColumn("Q2 WEEE from final holders (t)", x => x.FinalHolderQ2);
            csvWriter.DefineColumn("Q3 WEEE from final holders (t)", x => x.FinalHolderQ3);
            csvWriter.DefineColumn("Q4 WEEE from final holders (t)", x => x.FinalHolderQ4);
            csvWriter.DefineColumn("Total WEEE delivered (t)", x => x.DeliveredTotal);
            csvWriter.DefineColumn("Q1 WEEE delivered (t)", x => x.DeliveredQ1);
            csvWriter.DefineColumn("Q2 WEEE delivered (t)", x => x.DeliveredQ2);
            csvWriter.DefineColumn("Q3 WEEE delivered (t)", x => x.DeliveredQ3);
            csvWriter.DefineColumn("Q4 WEEE delivered (t)", x => x.DeliveredQ4);

            return csvWriter;
        }

        public IEnumerable<CsvResult> CreateResults(IEnumerable<DataReturn> dataReturns)
        {
            foreach (Domain.Obligation.ObligationType obligationType in
                new List<Domain.Obligation.ObligationType>() { Domain.Obligation.ObligationType.B2B, Domain.Obligation.ObligationType.B2C })
            {
                foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
                {
                    decimal? dcfQ1;
                    decimal? dcfQ2;
                    decimal? dcfQ3;
                    decimal? dcfQ4;
                    decimal? dcfTotal;

                    GetCollectedTotals(dataReturns, obligationType, category, WeeeCollectedAmountSourceType.Dcf,
                        out dcfQ1, out dcfQ2, out dcfQ3, out dcfQ4, out dcfTotal);

                    decimal? distributorQ1;
                    decimal? distributorQ2;
                    decimal? distributorQ3;
                    decimal? distributorQ4;
                    decimal? distributorTotal;

                    GetCollectedTotals(dataReturns, obligationType, category, WeeeCollectedAmountSourceType.Distributor,
                        out distributorQ1, out distributorQ2, out distributorQ3, out distributorQ4, out distributorTotal);

                    decimal? finalHolderQ1;
                    decimal? finalHolderQ2;
                    decimal? finalHolderQ3;
                    decimal? finalHolderQ4;
                    decimal? finalHolderTotal;

                    GetCollectedTotals(dataReturns, obligationType, category, WeeeCollectedAmountSourceType.FinalHolder,
                        out finalHolderQ1, out finalHolderQ2, out finalHolderQ3, out finalHolderQ4, out finalHolderTotal);

                    decimal? deliveredQ1;
                    decimal? deliveredQ2;
                    decimal? deliveredQ3;
                    decimal? deliveredQ4;
                    decimal? deliveredTotal;

                    GetDeliveredTotals(dataReturns, obligationType, category,
                        out deliveredQ1, out deliveredQ2, out deliveredQ3, out deliveredQ4, out deliveredTotal);

                    yield return new CsvResult(
                        category,
                        obligationType,
                        dcfTotal,
                        dcfQ1,
                        dcfQ2,
                        dcfQ3,
                        dcfQ4,
                        distributorTotal,
                        distributorQ1,
                        distributorQ2,
                        distributorQ3,
                        distributorQ4,
                        finalHolderTotal,
                        finalHolderQ1,
                        finalHolderQ2,
                        finalHolderQ3,
                        finalHolderQ4,
                        deliveredTotal,
                        deliveredQ1,
                        deliveredQ2,
                        deliveredQ3,
                        deliveredQ4);
                }
            }
        }

        private void GetCollectedTotals(
            IEnumerable<DataReturn> dataReturns,
            Domain.Obligation.ObligationType obligationType,
            WeeeCategory category,
            WeeeCollectedAmountSourceType source,
            out decimal? totalQ1,
            out decimal? totalQ2,
            out decimal? totalQ3,
            out decimal? totalQ4,
            out decimal? total)
        {
            total = null;

            IEnumerable<WeeeCollectedAmount> amountsQ1 = GetCollectedAmounts(dataReturns, obligationType, category, source, QuarterType.Q1);

            if (amountsQ1.Any())
            {
                totalQ1 = amountsQ1.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ1;
            }
            else
            {
                totalQ1 = null;
            }

            IEnumerable<WeeeCollectedAmount> amountsQ2 = GetCollectedAmounts(dataReturns, obligationType, category, source, QuarterType.Q2);

            if (amountsQ2.Any())
            {
                totalQ2 = amountsQ2.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ2;
            }
            else
            {
                totalQ2 = null;
            }

            IEnumerable<WeeeCollectedAmount> amountsQ3 = GetCollectedAmounts(dataReturns, obligationType, category, source, QuarterType.Q3);

            if (amountsQ3.Any())
            {
                totalQ3 = amountsQ3.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ3;
            }
            else
            {
                totalQ3 = null;
            }

            IEnumerable<WeeeCollectedAmount> amountsQ4 = GetCollectedAmounts(dataReturns, obligationType, category, source, QuarterType.Q4);

            if (amountsQ4.Any())
            {
                totalQ4 = amountsQ4.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ4;
            }
            else
            {
                totalQ4 = null;
            }
        }

        private void GetDeliveredTotals(
            IEnumerable<DataReturn> dataReturns,
            Domain.Obligation.ObligationType obligationType,
            WeeeCategory category,
            out decimal? totalQ1,
            out decimal? totalQ2,
            out decimal? totalQ3,
            out decimal? totalQ4,
            out decimal? total)
        {
            total = null;

            IEnumerable<WeeeDeliveredAmount> amountsQ1 = GetDeliveredAmounts(dataReturns, obligationType, category, QuarterType.Q1);

            if (amountsQ1.Any())
            {
                totalQ1 = amountsQ1.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ1;
            }
            else
            {
                totalQ1 = null;
            }

            IEnumerable<WeeeDeliveredAmount> amountsQ2 = GetDeliveredAmounts(dataReturns, obligationType, category, QuarterType.Q2);

            if (amountsQ2.Any())
            {
                totalQ2 = amountsQ2.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ2;
            }
            else
            {
                totalQ2 = null;
            }

            IEnumerable<WeeeDeliveredAmount> amountsQ3 = GetDeliveredAmounts(dataReturns, obligationType, category, QuarterType.Q3);

            if (amountsQ3.Any())
            {
                totalQ3 = amountsQ3.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ3;
            }
            else
            {
                totalQ3 = null;
            }

            IEnumerable<WeeeDeliveredAmount> amountsQ4 = GetDeliveredAmounts(dataReturns, obligationType, category, QuarterType.Q4);

            if (amountsQ4.Any())
            {
                totalQ4 = amountsQ4.Sum(x => x.Tonnage);
                total = (total ?? 0) + totalQ4;
            }
            else
            {
                totalQ4 = null;
            }
        }

        private IEnumerable<WeeeCollectedAmount> GetCollectedAmounts(
            IEnumerable<DataReturn> dataReturns,
            Domain.Obligation.ObligationType obligationType,
            WeeeCategory category,
            WeeeCollectedAmountSourceType source,
            QuarterType quarter)
        {
            return dataReturns
                .Where(dr => dr.Quarter.Q == quarter)
                .Where(dr => dr.CurrentVersion != null)
                .Where(dr => dr.CurrentVersion.WeeeCollectedReturnVersion != null)
                .SelectMany(dr => dr.CurrentVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts)
                .Where(wca => wca.ObligationType == obligationType)
                .Where(wca => wca.WeeeCategory == category)
                .Where(wca => wca.SourceType == source);
        }

        private IEnumerable<WeeeDeliveredAmount> GetDeliveredAmounts(
            IEnumerable<DataReturn> dataReturns,
            Domain.Obligation.ObligationType obligationType,
            WeeeCategory category,
            QuarterType quarter)
        {
            return dataReturns
                .Where(dr => dr.Quarter.Q == quarter)
                .Where(dr => dr.CurrentVersion != null)
                .Where(dr => dr.CurrentVersion.WeeeDeliveredReturnVersion != null)
                .SelectMany(dr => dr.CurrentVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts)
                .Where(wca => wca.ObligationType == obligationType)
                .Where(wca => wca.WeeeCategory == category);
        }

        public class CsvResult
        {
            public WeeeCategory Category { get; private set; }
            public Domain.Obligation.ObligationType ObligationType { get; private set; }
            public decimal? DcfTotal { get; private set; }
            public decimal? DcfQ1 { get; private set; }
            public decimal? DcfQ2 { get; private set; }
            public decimal? DcfQ3 { get; private set; }
            public decimal? DcfQ4 { get; private set; }
            public decimal? DistributorTotal { get; private set; }
            public decimal? DistributorQ1 { get; private set; }
            public decimal? DistributorQ2 { get; private set; }
            public decimal? DistributorQ3 { get; private set; }
            public decimal? DistributorQ4 { get; private set; }
            public decimal? FinalHolderTotal { get; private set; }
            public decimal? FinalHolderQ1 { get; private set; }
            public decimal? FinalHolderQ2 { get; private set; }
            public decimal? FinalHolderQ3 { get; private set; }
            public decimal? FinalHolderQ4 { get; private set; }
            public decimal? DeliveredTotal { get; private set; }
            public decimal? DeliveredQ1 { get; private set; }
            public decimal? DeliveredQ2 { get; private set; }
            public decimal? DeliveredQ3 { get; private set; }
            public decimal? DeliveredQ4 { get; private set; }

            public CsvResult(
                WeeeCategory category,
                Domain.Obligation.ObligationType obligationType,
                decimal? dcfTotal,
                decimal? dcfQ1,
                decimal? dcfQ2,
                decimal? dcfQ3,
                decimal? dcfQ4,
                decimal? distributorTotal,
                decimal? distributorQ1,
                decimal? distributorQ2,
                decimal? distributorQ3,
                decimal? distributorQ4,
                decimal? finalHolderTotal,
                decimal? finalHolderQ1,
                decimal? finalHolderQ2,
                decimal? finalHolderQ3,
                decimal? finalHolderQ4,
                decimal? deliveredTotal,
                decimal? deliveredQ1,
                decimal? deliveredQ2,
                decimal? deliveredQ3,
                decimal? deliveredQ4)
            {
                Category = category;
                ObligationType = obligationType;
                DcfTotal = dcfTotal;
                DcfQ1 = dcfQ1;
                DcfQ2 = dcfQ2;
                DcfQ3 = dcfQ3;
                DcfQ4 = dcfQ4;
                DistributorTotal = distributorTotal;
                DistributorQ1 = distributorQ1;
                DistributorQ2 = distributorQ2;
                DistributorQ3 = distributorQ3;
                DistributorQ4 = distributorQ4;
                FinalHolderTotal = finalHolderTotal;
                FinalHolderQ1 = finalHolderQ1;
                FinalHolderQ2 = finalHolderQ2;
                FinalHolderQ3 = finalHolderQ3;
                FinalHolderQ4 = finalHolderQ4;
                DeliveredTotal = deliveredTotal;
                DeliveredQ1 = deliveredQ1;
                DeliveredQ2 = deliveredQ2;
                DeliveredQ3 = deliveredQ3;
                DeliveredQ4 = deliveredQ4;
            }
        }
    }
}
