namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class SchemeMapping : EntityTypeConfiguration<Scheme>
    {
        public SchemeMapping()
        {
            ToTable("Scheme", "PCS");
        }
    }
}
