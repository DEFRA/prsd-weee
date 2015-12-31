namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class SystemDataMapping : EntityTypeConfiguration<SystemData>
    {
        public SystemDataMapping()
        {
            this.ToTable("SystemData", "dbo");

            Property(e => e.InitialIbisFileIdDatabaseValue).HasColumnName("InitialIbisFileId");
        }
    }
}