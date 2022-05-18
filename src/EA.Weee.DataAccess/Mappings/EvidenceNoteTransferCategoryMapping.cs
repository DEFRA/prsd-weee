namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    public class EvidenceNoteTransferCategoryMapping : EntityTypeConfiguration<NoteTransferCategory>
    {
        public EvidenceNoteTransferCategoryMapping()
        {
            ToTable("NoteTransferCategory", "Evidence");
            HasKey(n => n.Id);
            Property(n => n.CategoryId).IsRequired();
            
            HasRequired(a => a.TransferNote).WithMany(n => n.NoteTransferCategories).HasForeignKey(n => n.TransferNoteId);
        }
    }
}
