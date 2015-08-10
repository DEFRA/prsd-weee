namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class SchemeMapping : EntityTypeConfiguration<Scheme>
    {
        public SchemeMapping()
        {
            ToTable("Scheme", "PCS");
        }
    }
}
