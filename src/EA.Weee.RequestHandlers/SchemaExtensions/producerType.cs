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
        private ChargeBandType chargeBand;

        public virtual ChargeBandType GetProducerChargeBand()
        {
            if (chargeBand == null)
            {
                if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Lessthan5TEEEplacedonmarket)
                {
                    chargeBand = ChargeBandType.E;
                }
                else if (eeePlacedOnMarketBand == eeePlacedOnMarketBandType.Morethanorequalto5TEEEplacedonmarket)
                {
                    if (annualTurnoverBand == annualTurnoverBandType.Greaterthanonemillionpounds)
                    {
                        chargeBand = VATRegistered ? ChargeBandType.A : ChargeBandType.D;
                    }
                    else if (annualTurnoverBand == annualTurnoverBandType.Lessthanorequaltoonemillionpounds)
                    {
                        chargeBand = VATRegistered ? ChargeBandType.B : ChargeBandType.C;
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("Charge band for producer {0} could not be determined due to the annualTurnoverBand value of {1}.",
                            DeserializedXmlExtensions.GetProducerName(this), annualTurnoverBand));
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Charge band for producer {0} could not be determined due to the eeePlacedOnMarketBand value of {1}.",
                        DeserializedXmlExtensions.GetProducerName(this), eeePlacedOnMarketBand));
                }
            }

            return chargeBand;
        }
    }
}
