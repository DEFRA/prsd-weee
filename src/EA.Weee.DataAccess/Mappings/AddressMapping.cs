namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class AddressMapping : EntityTypeConfiguration<Address>
    {
        public AddressMapping()
        {
            ToTable("Address", "Organisation");

            //Property(x => x.Address1).HasColumnName("Address1").IsRequired().HasMaxLength(35);
            //Property(x => x.Address2).HasColumnName("Address2").HasMaxLength(35);
            //Property(x => x.TownOrCity).HasColumnName("TownOrCity").IsRequired().HasMaxLength(35);
            //Property(x => x.CountyOrRegion).HasColumnName("CountyOrRegion").HasMaxLength(35);
            //Property(x => x.Postcode).HasColumnName("Postcode").HasMaxLength(10);
            //Property(x => x.Country).HasColumnName("CountryId").HasMaxLength(10);
            //Property(x => x.Telephone).HasColumnName("Telephone").IsRequired().HasMaxLength(20);
            //Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(256);
            //this.Ignore(address => address.IsUkAddress);
        }
    }
}
