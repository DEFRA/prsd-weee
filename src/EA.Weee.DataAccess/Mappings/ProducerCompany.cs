namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerCompanyMapping : EntityTypeConfiguration<Company>
    {
        public ProducerCompanyMapping()
        {
            ToTable("Company", "Producer");
        }
    }
}
