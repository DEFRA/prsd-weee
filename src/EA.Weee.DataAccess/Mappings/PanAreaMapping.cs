namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Lookup;

    internal class PanAreaMapping : EntityTypeConfiguration<PanArea>
    {
        public PanAreaMapping()
        {
            this.ToTable("PanArea", "Lookup");
        }
    }
}
