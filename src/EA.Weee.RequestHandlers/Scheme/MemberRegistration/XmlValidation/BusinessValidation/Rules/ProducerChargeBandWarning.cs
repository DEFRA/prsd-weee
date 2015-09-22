namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProducerChargeBandWarning
    {
        public schemeType Scheme;
        public producerType Producer;
        public Guid OrganisationId;

        public ProducerChargeBandWarning(schemeType scheme, producerType producer, Guid organisationId)
        {
            Scheme = scheme;
            Producer = producer;
            OrganisationId = organisationId;
        }
    }
}
