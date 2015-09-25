namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
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
        private Dictionary<string, decimal> producerChargeBands;
        private Dictionary<ProducerYear, decimal> sumOfExistingChargesLookup;

        public ProducerChargeCalculatorDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        private void FetchData()
        {
            producerChargeBands = context
                .ProducerChargeBands
                .AsNoTracking()
                .DefaultIfEmpty()
                .ToDictionary(pcb => pcb.Name, pcb => pcb.Amount);

            sumOfExistingChargesLookup = context
                .Producers
                .Where(p => p.MemberUpload.IsSubmitted)
                .GroupBy(p => new
                {
                    RegistrationNumber = p.RegistrationNumber,
                    ComplianceYear = p.MemberUpload.ComplianceYear.Value
                })
                .Select(g => new
                {
                    Key = g.Key,
                    Total = g.Sum(i => i.ChargeThisUpdate)
                })
                .ToDictionary(
                    g => new ProducerYear(g.Key.RegistrationNumber, g.Key.ComplianceYear),
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

        public decimal FetchChargeBandAmount(ChargeBandType chargeBand)
        {
            EnsureDataFetched();

            return producerChargeBands[chargeBand.DisplayName];
        }

        public decimal FetchSumOfExistingCharges(string registrationNumber, int complianceYear)
        {
            ProducerYear key = new ProducerYear(registrationNumber, complianceYear);
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
            public string RegistrationNumber { get; private set; }
            public int ComplianceYear { get; private set; }

            public ProducerYear(string registrationNumber, int complianceYear)
                : this()
            {
                RegistrationNumber = registrationNumber;
                ComplianceYear = complianceYear;
            }
        }
    }
}