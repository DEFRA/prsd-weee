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
            Property(n => n.ChangedDate).IsRequired();
            Property(n => n.ChangedById).IsRequired();

            HasRequired(n => n.Note);
        }
    }
}
