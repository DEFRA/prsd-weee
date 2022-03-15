namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteStatusMapping : ComplexTypeConfiguration<NoteStatus>
    {
        public EvidenceNoteStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
