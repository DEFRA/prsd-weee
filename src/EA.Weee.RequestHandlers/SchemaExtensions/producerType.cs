namespace EA.Weee.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.Extensions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "This is the naming used in the XML file.")]
    public partial class producerType
    {
        public virtual ChargeBandType GetProducerChargeBand()
        {
            ChargeBandType chargeBand;

            if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
            {
                chargeBand = ChargeBandType.E;
            }
            else
            {
                if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds &&
                    VATRegistered
                    && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    chargeBand = ChargeBandType.A;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && VATRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    chargeBand = ChargeBandType.B;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds
                         && !VATRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    chargeBand = ChargeBandType.D;
                }
                else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds
                         && !VATRegistered
                         && eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    chargeBand = ChargeBandType.C;
                }
                else
                {
                    throw new ApplicationException(string.Format("Charge band for producer {0} could not be determined.", DeserializedXmlExtensions.GetProducerName(this)));
                }
            }

            return chargeBand;
        }
    }
}
