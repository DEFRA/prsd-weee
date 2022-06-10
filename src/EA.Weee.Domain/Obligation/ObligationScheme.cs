namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using CuttingEdge.Conditions;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;

    public partial class ObligationScheme : Entity
    {
        [ForeignKey("ObligationUploadId")]
        public virtual ObligationUpload ObligationUpload { get; private set; }

        public virtual Guid ObligationUploadId { get; private set; }

        public virtual int ComplianceYear { get; private set; }

        public virtual DateTime UpdatedDate { get; private set; }

        [ForeignKey("SchemeId")]
        public virtual Scheme Scheme { get; private set; }

        public virtual Guid SchemeId { get; private set; }

        public virtual ICollection<ObligationSchemeAmount> ObligationSchemeAmounts { get; private set; }

        protected ObligationScheme()
        {
            ObligationSchemeAmounts = new List<ObligationSchemeAmount>();
        }

        public ObligationScheme(Scheme scheme, int complianceYear)
        {
            Condition.Requires(scheme).IsNotNull();
            Condition.Requires(complianceYear).IsGreaterThan(0);

            Scheme = scheme;
            SchemeId = scheme.Id;
            ComplianceYear = complianceYear;
            ObligationSchemeAmounts = new List<ObligationSchemeAmount>();
            UpdatedDate = SystemTime.UtcNow;
        }

        public virtual void UpdateObligationUpload(ObligationUpload obligationUpload)
        {
            Condition.Requires(obligationUpload).IsNotNull();

            ObligationUpload = obligationUpload;
        }

        public virtual void UpdateObligationSchemeAmounts(List<ObligationSchemeAmount> updatedObligationSchemeAmounts)
        {
            bool obligationChanged = false;
            foreach (var updatedObligationSchemeAmount in updatedObligationSchemeAmounts)
            {
                var currentAmount =
                    this.ObligationSchemeAmounts.First(o => o.CategoryId == updatedObligationSchemeAmount.CategoryId);

                Condition.Requires(currentAmount)
                    .IsNotNull(
                        $"UpdateObligationSchemeAmounts failed to update {updatedObligationSchemeAmount.CategoryId} as not found in data set");

                var changed = currentAmount.UpdateObligation(updatedObligationSchemeAmount.Obligation);

                if (changed)
                {
                    obligationChanged = true;
                }
            }

            if (obligationChanged)
            {
                UpdatedDate = SystemTime.UtcNow;
            }
        }
    }
}
