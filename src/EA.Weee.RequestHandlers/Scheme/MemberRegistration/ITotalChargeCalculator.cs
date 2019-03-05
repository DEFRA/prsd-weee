namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using System.Collections.Generic;
    using Domain.Scheme;
    using Requests.Scheme.MemberRegistration;

    public interface ITotalChargeCalculator
    {
        Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges);
    }
}
