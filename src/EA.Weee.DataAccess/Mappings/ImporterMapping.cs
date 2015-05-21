namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Notification;

    internal class ImporterMapping : EntityTypeConfiguration<Importer>
    {
        public ImporterMapping()
        {
            this.ToTable("Importer", "Business");
        }
    }
}
