namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Evidence;
    using System.Data.Entity.ModelConfiguration;

    internal class NoteTypeMapping : ComplexTypeConfiguration<NoteType>
    {
        public NoteTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("NoteType");
        }
    }
}
