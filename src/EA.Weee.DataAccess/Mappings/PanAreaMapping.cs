namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain;
    using System.Data.Entity.ModelConfiguration;

    internal class PanAreaMapping : EntityTypeConfiguration<PanArea>
    {
        public PanAreaMapping()
        {
            this.ToTable("PanArea", "Lookup");
        }
    }
}
