namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;

    public class ProducerNameWarning
    {
        public schemeType Scheme;
        public producerType Producer;
        public Guid OrganisationId;

        public ProducerNameWarning(schemeType scheme, producerType producer, Guid organisationId)
        {
            Scheme = scheme;
            Producer = producer;
            OrganisationId = organisationId;
        }
    }
}
