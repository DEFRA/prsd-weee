namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProducerChargeCalculatorDataAccess : IProducerChargeCalculatorDataAccess
    {
        private readonly WeeeContext context;

        private bool dataFetched = false;
        private Dictionary<ChargeBand, ChargeBandAmount> currentProducerChargeBandAmounts;
        private Dictionary<ProducerYear, decimal> sumOfExistingChargesLookup;

        public ProducerChargeCalculatorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        private void FetchData()
        {
            /* For now we only have one charge band amount for each type, so
             * we can fetch them all. When new charge band amounts are added,
             * this query will need to select only latest charge band amount
             * for each charge band type.
             */
            currentProducerChargeBandAmounts = context
                .ChargeBandAmounts
                .ToDictionary(pcb => pcb.ChargeBand, pcb => pcb);

            sumOfExistingChargesLookup = context
                .Producers
                .Where(p => p.MemberUpload.IsSubmitted)
                .GroupBy(p => new
                {
                    SchemeApprovalNumber = p.Scheme.ApprovalNumber,
                    RegistrationNumber = p.RegistrationNumber,
                    ComplianceYear = p.MemberUpload.ComplianceYear.Value
                })
                .Select(g => new
                {
                    Key = g.Key,
                    Total = g.Sum(i => i.ChargeThisUpdate)
                })
                .ToDictionary(
                    g => new ProducerYear(g.Key.SchemeApprovalNumber, g.Key.RegistrationNumber, g.Key.ComplianceYear),
                    g => g.Total);
        }

        private void EnsureDataFetched()
        {
            if (!dataFetched)
            {
                FetchData();
                dataFetched = true;
            }
        }

        public ChargeBandAmount FetchCurrentChargeBandAmount(ChargeBand chargeBandType)
        {
            EnsureDataFetched();

            return currentProducerChargeBandAmounts[chargeBandType];
        }

        public decimal FetchSumOfExistingCharges(string schemeApprovalNumber, string registrationNumber, int complianceYear)
        {
            EnsureDataFetched();

            ProducerYear key = new ProducerYear(schemeApprovalNumber, registrationNumber, complianceYear);
            if (sumOfExistingChargesLookup.ContainsKey(key))
            {
                return sumOfExistingChargesLookup[key];
            }
            else
            {
                return 0;
            }
        }

        private struct ProducerYear
        {
            public string SchemeApprovalNumber { get; private set; }
            public string RegistrationNumber { get; private set; }
            public int ComplianceYear { get; private set; }

            public ProducerYear(string schemeApprovalNumber, string registrationNumber, int complianceYear)
                : this()
            {
                SchemeApprovalNumber = schemeApprovalNumber;
                RegistrationNumber = registrationNumber;
                ComplianceYear = complianceYear;
            }
        }
    }
}