namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using Domain.Scheme;
    using RequestHandlers.Scheme.MemberRegistration;
    public interface IMigrationTotalChargeCalculatorDataAccess : ITotalChargeCalculatorDataAccess
    {
        bool CheckSchemeHasAnnualCharge(Scheme scheme, int year, DateTime date);
    }
}
