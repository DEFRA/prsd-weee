namespace EA.Weee.Xml.MemberRegistration
{
    using Domain;
    
    public static class Extensions
    {
        public static string GetProducerName(this producerType producer)
        {
            if (producer.producerBusiness != null && producer.producerBusiness.Item != null)
            {
                var producerItem = producer.producerBusiness.Item;

                if (producerItem.GetType() == typeof(companyType))
                {
                    return ((companyType)producerItem).companyName ?? producer.tradingName;
                }

                if (producerItem.GetType() == typeof(partnershipType))
                {
                    return ((partnershipType)producerItem).partnershipName ?? producer.tradingName;
                }
            }

            return producer.tradingName;
        }

        public static ObligationType ToDomainObligationType(this obligationTypeType obligationType)
        {
            switch (obligationType)
            {
                case obligationTypeType.B2B:
                    return ObligationType.B2B;
                case obligationTypeType.B2C:
                    return ObligationType.B2C;

                default:
                    return ObligationType.Both;
            }
        }

        public static obligationTypeType ToDeserializedXmlObligationType(ObligationType obligationType)
        {
            if (obligationType == ObligationType.B2B)
            {
                return obligationTypeType.B2B;
            }

            if (obligationType == ObligationType.B2C)
            {
                return obligationTypeType.B2C;
            }

            return obligationTypeType.Both;
        }
    }
}
