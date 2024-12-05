﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Evidence;
    using System.Data.Entity.ModelConfiguration;

    internal class EvidenceNoteStatusHistoryMapping : EntityTypeConfiguration<NoteStatusHistory>
    {
        public EvidenceNoteStatusHistoryMapping()
        {
            ToTable("NoteStatusHistory", "Evidence");
            HasKey(n => n.Id);
            Property(n => n.ToStatus.Value).HasColumnName("ToStatus").IsRequired();
            Property(n => n.FromStatus.Value).HasColumnName("FromStatus").IsRequired();
            Property(n => n.ChangedDate).HasColumnName("ChangedDate").IsRequired();
            Property(n => n.ChangedById).HasColumnName("ChangedById").IsRequired();
            Property(n => n.Reason).HasColumnName("Reason").IsOptional().IsUnicode().HasMaxLength(200);

            HasRequired(n => n.Note);
        }
    }
}
