namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Lookup;
    using System.Data.Entity.ModelConfiguration;

    internal class LocalAreaMapping : EntityTypeConfiguration<LocalArea>
    {
        public LocalAreaMapping()
        {
            this.ToTable("LocalArea", "Lookup");
        }
    }
}
