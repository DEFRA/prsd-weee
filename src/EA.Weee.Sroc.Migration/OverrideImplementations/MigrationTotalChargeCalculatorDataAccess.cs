namespace EA.Weee.Sroc.Migration.OverrideImplementations
{
    using System;
    using System.Linq;
    using Domain.Scheme;

    public class MigrationTotalChargeCalculatorDataAccess : IMigrationTotalChargeCalculatorDataAccess
    {
        private readonly WeeeMigrationContext context;

        public MigrationTotalChargeCalculatorDataAccess(WeeeMigrationContext context)
        {
            this.context = context;
        }

        public bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckSchemeHasAnnualCharge(Scheme scheme, int year, DateTime date)
        {
            return context.MemberUploads.Any(m => m.HasAnnualCharge
                                                  && m.Scheme.OrganisationId == scheme.OrganisationId
                                                  && m.ComplianceYear == year
                                                  && m.IsSubmitted
                                                  && m.SubmittedDate < date);
        }
    }
}
