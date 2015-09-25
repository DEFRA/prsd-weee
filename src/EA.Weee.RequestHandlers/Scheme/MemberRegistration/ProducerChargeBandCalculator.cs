namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using RequestHandlers;
    using Xml.Schemas;

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
                var producerRecordsSoFarThisYear =
                    context.Producers.Where(
                        p => p.RegistrationNumber == producer.registrationNo
                          && p.MemberUpload.ComplianceYear == complianceYear
                          && p.MemberUpload.IsSubmitted);

                var chargesSoFarThisYear = producerRecordsSoFarThisYear.Select(p => p.ChargeThisUpdate);

                var totalChargeSoFarThisYear = chargesSoFarThisYear.DefaultIfEmpty(0).Sum();

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
            ChargeBandType chargeBandType = GetProducerChargeBand(
                producer.annualTurnoverBand,
                producer.VATRegistered,
                producer.eeePlacedOnMarketBand);

            decimal chargeAmount = GetChargeAmount(chargeBandType);

            return new ProducerCharge()
            {
                ChargeBandType = chargeBandType,
                ChargeAmount = chargeAmount
            };
        }

        public static ChargeBandType GetProducerChargeBand(
            annualTurnoverBandType annualTurnoverBand,
            bool vatRegistered,
            eeePlacedOnMarketBandType eeePlacedOnMarketBand)
        {
            if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return ChargeBandType.E;
            }
            else
            {
                if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && vatRegistered
                    && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBandType.A;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBandType.B;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBandType.D;
                }
                else
                {
                    return ChargeBandType.C;
                }
            }
        }
    }
}
