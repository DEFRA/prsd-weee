namespace EA.Weee.DataAccess.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteTonnageMapping : EntityTypeConfiguration<NoteTonnage>
    {
        public EvidenceNoteTonnageMapping()
        {
            ToTable("NoteTonnage", "Evidence");
            HasKey(n => n.Id);
            Property(n => n.CategoryId).IsRequired();
            Property(n => n.NoteId).IsRequired();
            Property(n => n.Received).IsOptional().HasPrecision(28, 3);
            Property(n => n.Reused).IsOptional().HasPrecision(28, 3);

            HasRequired(a => a.Note).WithMany(b => b.NoteTonnage).HasForeignKey(c => c.NoteId);
        }
    }
}
