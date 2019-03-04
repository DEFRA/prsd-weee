namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;
    public interface ITotalChargeCalculator
    {
        Dictionary<string, ProducerCharge> TotalCalculatedCharges(ProcessXmlFile message, Scheme scheme, int complianceYear, ref bool hasAnnualCharge, ref decimal? totalCharges);
    }
}
