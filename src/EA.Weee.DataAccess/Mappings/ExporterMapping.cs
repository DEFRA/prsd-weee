namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Notification;

    internal class ExporterMapping : EntityTypeConfiguration<Exporter>
    {
        public ExporterMapping()
        {
            ToTable("Exporter", "Notification");
        }
    }
}
