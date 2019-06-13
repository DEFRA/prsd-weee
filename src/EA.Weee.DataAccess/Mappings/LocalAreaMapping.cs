namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Admin;

    internal class LocalAreaMapping : EntityTypeConfiguration<LocalArea>
    {
        public LocalAreaMapping()
        {
            this.ToTable("LocalArea", "Lookup");
        }
    }
}
