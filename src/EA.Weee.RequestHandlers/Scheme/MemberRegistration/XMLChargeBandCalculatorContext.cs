namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Domain.Error;
    using Domain.Scheme;
    using EA.Weee.DataAccess;
    using Interfaces;
    using Requests.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.MemberRegistration;
    public class XMLChargeBandCalculatorContext
    {
        private IXMLChargeBandCalculator xmlChargeBandCalculator;

        public XMLChargeBandCalculatorContext()
        {
        }

        public void SetChargeBandCalculatorContext(IXMLChargeBandCalculator xmlChargeBandCalculator)
        {
            this.xmlChargeBandCalculator = xmlChargeBandCalculator;
        }

        public Dictionary<string, ProducerCharge> SetChargeBandCalculator(ProcessXmlFile file)
        {
            var producerCharges = xmlChargeBandCalculator.Calculate(file);
            return producerCharges;
        }
    }
}
