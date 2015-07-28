namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Domain;
    using PCS;
    using Prsd.Core.Domain;
    using RequestHandlers;

    public class ProducerChargeBandCalculator
    {
        public decimal CalculateCharge(producerType[] producers)
        {
            var totalCharges = 0;
            foreach (var producer in producers)
            {
                if (producer.status == statusType.I)
                {
                    var chargeBand = string.Empty;

                    if (producer.eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
                    {
                        chargeBand = "E";
                        totalCharges = totalCharges + 30;
                    }
                    else
                    {
                        if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                            && producer.VATRegistered
                            && producer.eeePlacedOnMarketBand ==
                            eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                        {
                            chargeBand = "A";
                            totalCharges = totalCharges + 445;
                        }
                        else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                                 && producer.VATRegistered
                                 && producer.eeePlacedOnMarketBand ==
                                 eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                        {
                            chargeBand = "B";
                            totalCharges = totalCharges + 210;
                        }
                        else if (producer.annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                                 && producer.VATRegistered == false
                                 && producer.eeePlacedOnMarketBand ==
                                 eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                        {
                            chargeBand = "D";
                            totalCharges = totalCharges + 30;
                        }
                        else if (producer.annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                                 && producer.VATRegistered == false
                                 && producer.eeePlacedOnMarketBand ==
                                 eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                        {
                            chargeBand = "C";
                            totalCharges = totalCharges + 30;
                        }
                    }
                }
            }
            return totalCharges;
        }
    }
}
