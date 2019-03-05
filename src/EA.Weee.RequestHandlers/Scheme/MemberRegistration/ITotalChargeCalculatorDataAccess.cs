namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;

    public interface ITotalChargeCalculatorDataAccess
    {
        bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear);
    }
}
