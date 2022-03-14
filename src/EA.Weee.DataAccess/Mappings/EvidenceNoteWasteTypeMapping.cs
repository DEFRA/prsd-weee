namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteWasteTypeMapping : ComplexTypeConfiguration<WasteType>
    {
        public EvidenceNoteWasteTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("WasteType");
        }
    }
}
