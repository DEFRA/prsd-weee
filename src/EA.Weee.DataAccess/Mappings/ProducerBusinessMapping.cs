namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerBusinessMapping : EntityTypeConfiguration<ProducerBusiness>
    {
        public ProducerBusinessMapping()
        {
            ToTable("Business", "Producer");
        }
    }
}