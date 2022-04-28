namespace EA.Weee.DataAccess.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

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
            Property(n => n.Reason).HasColumnName("Reason").IsOptional().IsUnicode().HasMaxLength(2000);

            HasRequired(n => n.Note);
        }
    }
}
