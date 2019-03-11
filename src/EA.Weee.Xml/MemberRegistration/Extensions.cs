﻿namespace EA.Weee.Xml.MemberRegistration
{
    using Domain.Obligation;

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

        public static countryType GetProducerCountry(this producerType producer)
        {
            countryType countryType = new countryType();

            if (producer.producerBusiness != null && producer.producerBusiness.Item != null)
            {
                var producerItem = producer.producerBusiness.Item;

                if (producerItem.GetType() == typeof(companyType))
                {
                    var producerRegisteredOffice = ((companyType)producerItem).registeredOffice;
                    return countryType = producerRegisteredOffice.contactDetails.address.country;
                }

                if (producerItem.GetType() == typeof(partnershipType))
                {
                    var producerPrincipalBusiness = ((partnershipType)producerItem).principalPlaceOfBusiness;
                    return countryType = producerPrincipalBusiness.contactDetails.address.country;
                }
            }
            return countryType;
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

        public static obligationTypeType ToDeserializedXmlObligationType(this ObligationType obligationType)
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
