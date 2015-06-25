namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class AddressMapping : EntityTypeConfiguration<Address>
    {
        public AddressMapping()
        {
            ToTable("Address", "Organisation");
        }
    }
}
