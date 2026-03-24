namespace EA.Weee.Domain.Lookup
{
    using EA.Prsd.Core.Domain;
    using System;

    /// <summary>
    /// Represents the annual subsistence charge for a Competent Authority 
    /// for a specific compliance year.
    /// </summary>
    public class AnnualChargeByYear : Entity
    {
        public Guid CompetentAuthorityId { get; private set; }

        public int ComplianceYear { get; private set; }

        public decimal AnnualChargeAmount { get; private set; }

        public DateTime? EffectiveFrom { get; private set; }

        public virtual UKCompetentAuthority CompetentAuthority { get; private set; }

        protected AnnualChargeByYear()
        {
        }

        public AnnualChargeByYear(
            Guid competentAuthorityId,
            int complianceYear,
            decimal annualChargeAmount,
            DateTime? effectiveFrom)
        {
            CompetentAuthorityId = competentAuthorityId;
            ComplianceYear = complianceYear;
            AnnualChargeAmount = annualChargeAmount;
            EffectiveFrom = effectiveFrom;
        }
    }
}
