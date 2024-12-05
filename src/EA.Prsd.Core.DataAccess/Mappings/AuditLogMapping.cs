﻿namespace EA.Prsd.Core.DataAccess.Mappings
{
    using EA.Prsd.Core.Domain.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class AuditLogMapping : EntityTypeConfiguration<AuditLog>
    {
        public AuditLogMapping()
        {
            ToTable("AuditLog", "Auditing");

            Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.UserId)
                .IsRequired();

            Property(x => x.EventDate)
                .IsRequired();

            Property(x => x.EventType)
                .IsRequired();

            Property(x => x.TableName)
                .HasMaxLength(256)
                .IsRequired();

            Property(x => x.RecordId)
                .IsRequired();
        }
    }
}