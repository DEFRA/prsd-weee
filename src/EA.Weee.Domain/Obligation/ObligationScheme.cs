namespace EA.Weee.Domain.Obligation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using CuttingEdge.Conditions;
    using Lookup;
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

        public ObligationScheme()
        {
        }

        public ObligationScheme(Scheme scheme, int complianceYear)
        {
            Condition.Requires(scheme).IsNotNull();
            Condition.Requires(complianceYear).IsGreaterThan(0);

            Scheme = scheme;
            ComplianceYear = complianceYear;
            ObligationSchemeAmounts = new List<ObligationSchemeAmount>();
            UpdatedDate = SystemTime.UtcNow;
        }

        public virtual void SetUpdatedDate(DateTime date)
        {
            //only update the updated date if the value is different to the current value.
            UpdatedDate = date;
        }
    }
}
