namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;

    internal class PanAreaMapping : EntityTypeConfiguration<PanArea>
    {
        public PanAreaMapping()
        {
            this.ToTable("PanArea", "Lookup");
        }
    }
}
