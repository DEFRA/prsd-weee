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

    public interface IProducerChargeBandCalculator
    {
        ChargeBand GetProducerChargeBand(annualTurnoverBandType annualTurnoverBand, bool vatRegistered, eeePlacedOnMarketBandType eeePlacedOnMarketBand);
    }
}
