namespace EA.Weee.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Xml.Schemas;

    public class ProducerChargeBandCalculator : IProducerChargeBandCalculator
    {
        public ChargeBandType GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand)
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
