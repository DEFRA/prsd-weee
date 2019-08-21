namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Domain.Scheme;

    public interface ITotalChargeCalculatorDataAccess
    {
        bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear);
    }
}
