namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Evidence;
    using System.Data.Entity.ModelConfiguration;

    internal class EvidenceNoteStatusMapping : ComplexTypeConfiguration<NoteStatus>
    {
        public EvidenceNoteStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
