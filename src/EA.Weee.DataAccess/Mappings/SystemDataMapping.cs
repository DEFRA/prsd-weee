namespace EA.Weee.DataAccess.Mappings
{
    using Domain;
    using System.Data.Entity.ModelConfiguration;

    internal class SystemDataMapping : EntityTypeConfiguration<SystemData>
    {
        public SystemDataMapping()
        {
            this.ToTable("SystemData", "dbo");
        }
    }
}