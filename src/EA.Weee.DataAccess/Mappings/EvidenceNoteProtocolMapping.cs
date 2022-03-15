namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class EvidenceNoteProtocolMapping : ComplexTypeConfiguration<Protocol>
    {
        public EvidenceNoteProtocolMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Protocol");
        }
    }
}
