namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Evidence;

    internal class NoteTypeMapping : ComplexTypeConfiguration<NoteType>
    {
        public NoteTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("NoteType");
        }
    }
}
