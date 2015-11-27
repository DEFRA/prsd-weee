namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerSISCodeMapping : EntityTypeConfiguration<SICCode>
    {
        public ProducerSISCodeMapping()
        {
            ToTable("SICCode", "Producer");
        }
    }
}