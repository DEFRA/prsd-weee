namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;

    public class ProducerAlreadyRegisteredError
    {
        public schemeType Scheme;
        public producerType Producer;
        public Guid OrganisationId;

        public ProducerAlreadyRegisteredError(schemeType scheme, producerType producer, Guid orgId)
        {
            Scheme = scheme;
            Producer = producer;
            OrganisationId = orgId;
        }
    }
}
