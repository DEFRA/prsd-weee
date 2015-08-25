namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Prsd.Core.Domain;
    using RequestHandlers;

    public class ProducerChargeBandCalculator
    {
        private readonly WeeeContext context;
        private readonly List<ProducerChargeBand> producerChargeBands;

        public ProducerChargeBandCalculator(WeeeContext context)
        {
            this.context = context;
            producerChargeBands = context.ProducerChargeBands.ToList();
        }

        public ProducerCharge CalculateCharge(producerType producer, int complianceYear)
        {
            var producerCharge = GetProducerCharge(producer);

            if (producer.status == statusType.A)
            {
                var chargesSoFarThisYear =
                    context.Producers.Where(p => p.RegistrationNumber == producer.registrationNo
                        && p.MemberUpload.ComplianceYear == complianceYear
                        && p.MemberUpload.IsSubmitted)
                        .Select(p => p.ChargeThisUpdate)
                        .ToList();

                if (chargesSoFarThisYear.Count == 0)
                {
                    throw new ArgumentException(
                        string.Format(
                            "Expected at least one previous submitted producer record for amended producer {0} and compliance year {1}, but found zero",
                            producer.registrationNo, complianceYear));
                }

                decimal totalChargeSoFarThisYear = chargesSoFarThisYear.Sum();

                producerCharge.ChargeAmount = Math.Max(0, producerCharge.ChargeAmount - totalChargeSoFarThisYear);
            }

            return producerCharge;
        }

        public decimal GetChargeAmount(ChargeBandType chargeBandType)
        {
            return producerChargeBands.Single(pc => pc.Name == chargeBandType.DisplayName).Amount;
        }

        private ProducerCharge GetProducerCharge(producerType producer)
        {
            ProducerCharge producerCharge = new ProducerCharge();

            if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                producerCharge.ChargeBandType = ChargeBandType.E;
                producerCharge.ChargeAmount = GetChargeAmount(ChargeBandType.E);
            }
            else
            {
                if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds && producer.VATRegistered
                    && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.A;
                    producerCharge.ChargeAmount = GetChargeAmount(ChargeBandType.A);
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && producer.VATRegistered
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.B;
                    producerCharge.ChargeAmount = GetChargeAmount(ChargeBandType.B);
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && producer.VATRegistered == false
                         && producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.D;
                    producerCharge.ChargeAmount = GetChargeAmount(ChargeBandType.D);
                }
                else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && producer.VATRegistered == false
                         && producer.eeePlacedOnMarketBand
                         == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    producerCharge.ChargeBandType = ChargeBandType.C;
                    producerCharge.ChargeAmount = GetChargeAmount(ChargeBandType.C);
                }
            }

            return producerCharge;
        }

        private ChargeBandType GetChargeBandEnumeration(int value)
        {
            return Enumeration.FromValue<ChargeBandType>(value);
        }
    }
}
