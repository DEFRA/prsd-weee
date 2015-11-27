namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerBusinessMapping : EntityTypeConfiguration<ProducerBusiness>
    {
        public ProducerBusinessMapping()
        {
            ToTable("Business", "Producer");
        }
    }
}