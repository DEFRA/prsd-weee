namespace EA.Weee.DataAccess.Mappings
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteTransferMapping : EntityTypeConfiguration<NoteTransferTonnage>
    {
        public EvidenceNoteTransferMapping()
        {
            ToTable("NoteTransferTonnage", "Evidence");
            HasKey(n => n.Id);
            Property(n => n.TransferNoteId).IsRequired();
            Property(n => n.NoteTonnageId).IsRequired();
            Property(n => n.Received).IsOptional().HasPrecision(28, 3);
            Property(n => n.Reused).IsOptional().HasPrecision(28, 3);

            HasRequired(n => n.TransferNote).WithMany(n1 => n1.NoteTransferTonnage)
                .HasForeignKey(n2 => n2.TransferNoteId);
        }
    }
}
