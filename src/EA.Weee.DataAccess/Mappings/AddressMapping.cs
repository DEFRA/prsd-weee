namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class AddressMapping : EntityTypeConfiguration<Address>
    {
        public AddressMapping()
        {
            ToTable("Address", "Organisation");

            Property(x => x.Address1).HasColumnName("Address1").IsRequired();
            Property(x => x.Address2).HasColumnName("Address2");
            Property(x => x.TownOrCity).HasColumnName("TownOrCity").IsRequired();
            Property(x => x.CountyOrRegion).HasColumnName("CountyOrRegion");
            Property(x => x.PostalCode).HasColumnName("PostalCode").IsRequired();
            Property(x => x.Country).HasColumnName("Country").IsRequired();
            Property(x => x.TelePhone).HasColumnName("TelePhone").IsRequired();
            Property(x => x.Email).HasColumnName("Email").IsRequired();
            this.Ignore(address => address.IsUkAddress);
        }
    }
}
