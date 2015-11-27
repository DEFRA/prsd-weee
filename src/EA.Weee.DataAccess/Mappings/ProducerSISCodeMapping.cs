namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerSISCodeMapping : EntityTypeConfiguration<SICCode>
    {
        public ProducerSISCodeMapping()
        {
            ToTable("SICCode", "Producer");
        }
    }
}