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

            HasRequired(n => n.NoteTonnage);
            HasRequired(n => n.TransferNote);
        }
    }
}
