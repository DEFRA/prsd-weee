namespace EA.Weee.Domain.Audit
{
    using System;
    using Prsd.Core.Domain;

    public abstract class AuditableEntity : Entity, IAuditableEntity
    {
        public DateTime? Date { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; private set; }
    }
}
