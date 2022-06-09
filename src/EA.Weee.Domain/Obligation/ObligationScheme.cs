namespace EA.Weee.Domain.Obligation
{
    using System;
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

        public virtual WeeeCategory CategoryId { get; private set; }

        public virtual decimal? Obligation { get; private set; }

        public ObligationScheme(WeeeCategory categoryId, Scheme scheme, decimal? obligation, int complianceYear)
        {
            Condition.Requires(scheme).IsNotNull();
            Condition.Requires(complianceYear).IsGreaterThan(0);

            CategoryId = categoryId;
            Scheme = scheme;
            Obligation = obligation;
            ComplianceYear = complianceYear;
            UpdatedDate = SystemTime.UtcNow;
        }
    }
}
