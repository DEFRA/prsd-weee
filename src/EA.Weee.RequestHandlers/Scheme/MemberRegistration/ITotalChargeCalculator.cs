﻿namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    using Xml.MemberRegistration;

    public interface ITotalChargeCalculator
    {
        Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, bool annualChargeToBeAdded, ref decimal? totalCharges);
    }
}
