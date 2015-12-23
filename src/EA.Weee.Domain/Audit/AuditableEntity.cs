namespace EA.Weee.Domain.Audit
{
    using System;
    using Prsd.Core.Domain;

    public abstract class AuditableEntity : Entity
    {
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual User CreatedBy { get; set; }

        public string CreatedById { get; set; }

        public virtual User UpdatedBy { get; set; }

        public string UpdatedById { get; set; }
    }
}
