namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class AddressMapping : ComplexTypeConfiguration<Address>
    {
        public AddressMapping()
        {
            Property(x => x.Building).HasColumnName("Building");
            Property(x => x.Address1).HasColumnName("Address1");
            Property(x => x.Address2).HasColumnName("Address2");
            Property(x => x.TownOrCity).HasColumnName("TownOrCity");
            Property(x => x.PostalCode).HasColumnName("PostalCode");
            Property(x => x.Country).HasColumnName("Country");

            this.Ignore(address => address.IsUkAddress);
        }
    }
}
