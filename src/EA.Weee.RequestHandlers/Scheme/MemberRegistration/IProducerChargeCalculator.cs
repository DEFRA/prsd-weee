﻿namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Xml.Schemas;

    public interface IProducerChargeCalculator
    {
        ProducerCharge CalculateCharge(producerType producer, int complianceYear);
    }
}
