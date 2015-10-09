namespace EA.Weee.Domain.Audit
{
    using System;

    public interface IAuditableEntity
    {
        DateTime? Date { get; set; }
        string UserId { get; set; }
    }
}
