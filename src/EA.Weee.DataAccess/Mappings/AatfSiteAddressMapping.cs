namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfSiteAddressMapping : EntityTypeConfiguration<AatfSiteAddress>
    {
        public AatfSiteAddressMapping()
        {
            ToTable("Address", "AATF");

            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.Address1).HasColumnName("Address1").IsRequired().HasMaxLength(35);
            Property(x => x.Address2).HasColumnName("Address2").HasMaxLength(35);
            Property(x => x.TownOrCity).HasColumnName("TownOrCity").IsRequired().HasMaxLength(35);
            Property(x => x.CountyOrRegion).HasColumnName("CountyOrRegion").HasMaxLength(35);
            Property(x => x.CountyOrRegion).HasColumnName("Postcode").HasMaxLength(10);
            Property(x => x.CountryId).HasColumnName("CountryId").IsRequired();
        }
    }
}
