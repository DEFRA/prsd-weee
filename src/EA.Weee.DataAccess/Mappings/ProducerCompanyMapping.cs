namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerCompanyMapping : EntityTypeConfiguration<Company>
    {
        public ProducerCompanyMapping()
        {
            ToTable("Company", "Producer");
        }
    }
}
