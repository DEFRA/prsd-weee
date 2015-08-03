namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerChargeBandMapping : EntityTypeConfiguration<ProducerChargeBand>
    {
        public ProducerChargeBandMapping()
        {
            ToTable("ProducerChargeBand", "Producer");
        }
    }
}
