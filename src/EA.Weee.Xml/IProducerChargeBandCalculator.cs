namespace EA.Weee.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Xml.Schemas;

    public interface IProducerChargeBandCalculator
    {
        ChargeBandType GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand);
    }
}
