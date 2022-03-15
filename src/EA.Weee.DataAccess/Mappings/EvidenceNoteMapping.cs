namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteMapping : EntityTypeConfiguration<Note>
    {
        public EvidenceNoteMapping()
        {
            ToTable("Note", "Evidence");
        }
    }
}
