namespace EA.Weee.Xml
{
    using Domain.Lookup;
    using EA.Weee.Domain;
    using EA.Weee.Xml.Schemas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public ChargeBand GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand)
        {
            if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                return ChargeBand.E;
            }
            else
            {
                if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                    && vatRegistered
                    && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.A;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.B;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !vatRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    return ChargeBand.D;
                }
                else
                {
                    return ChargeBand.C;
                }
            }
        }
    }
}
