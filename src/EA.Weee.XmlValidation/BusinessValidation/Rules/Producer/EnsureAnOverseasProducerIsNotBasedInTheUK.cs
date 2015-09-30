namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using EA.Weee.Xml.Schemas;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml;

    /// <summary>
    /// This rules ensures that an overseas producer is not based in the UK.
    /// </summary>
    public class EnsureAnOverseasProducerIsNotBasedInTheUK : IEnsureAnOverseasProducerIsNotBasedInTheUK
    {
        public RuleResult Evaluate(producerType producer)
        {
            if (producer.authorisedRepresentative == null)
            {
                return RuleResult.Pass();
            }

            if (producer.authorisedRepresentative.overseasProducer == null)
            {
                return RuleResult.Pass();
            }

            if (producer.authorisedRepresentative.overseasProducer.overseasContact == null)
            {
                return RuleResult.Pass();
            }

            contactDetailsType contact = producer.authorisedRepresentative.overseasProducer.overseasContact;

            switch (contact.address.country)
            {
                case countryType.UKENGLAND:
                case countryType.UKNORTHERNIRELAND:
                case countryType.UKSCOTLAND:
                case countryType.UKWALES:

                    string errorMessage = string.Format(
                        "{0} {1} is an Authorised Representative but has a country named in their overseas producers' address details which is in the UK. " +
                        "An Authorised Representative cannot represent a UK based producer. In order to register this producer, please check whether " + 
                        "they are an Authorised Representative and that their client is not based in the UK. You will need to amend their details in " + 
                        "the XML accordingly.",
                        producer.GetProducerName(),
                        producer.registrationNo);

                    return RuleResult.Fail(errorMessage, Core.Shared.ErrorLevel.Error);

                default:
                    return RuleResult.Pass();
            }
        }
    }
}
