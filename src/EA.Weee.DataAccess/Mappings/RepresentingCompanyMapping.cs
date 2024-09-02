namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.Organisation;

    public class RepresentingCompanyConfiguration : EntityTypeConfiguration<RepresentingCompany>
    {
        public RepresentingCompanyConfiguration()
        {
            ToTable("RepresentingCompany", "Organisation");

            HasKey(e => e.Id);
            Property(o => o.CompanyName).HasColumnName("Name").HasMaxLength(256).IsRequired();
            Property(o => o.TradingName).HasColumnName("TradingName").HasMaxLength(256).IsOptional();
            Property(o => o.Address1).HasColumnName("Address1").HasMaxLength(60).IsRequired();
            Property(o => o.Address2).HasColumnName("Address2").HasMaxLength(60).IsOptional();
            Property(o => o.TownOrCity).HasColumnName("TownOrCity").HasMaxLength(35).IsRequired();
            Property(o => o.CountyOrRegion).HasColumnName("CountyOrRegion").HasMaxLength(35).IsOptional();
            Property(o => o.Postcode).HasColumnName("Postcode").HasMaxLength(10).IsRequired();
            Property(o => o.CountryId).HasColumnName("CountryId").IsRequired();
            Property(o => o.Telephone).HasColumnName("Telephone").HasMaxLength(20).IsRequired();
            Property(o => o.Email).HasColumnName("Email").HasMaxLength(256).IsRequired();
            Property(e => e.RowVersion).IsRowVersion();
            
            HasRequired(e => e.Country).WithMany().HasForeignKey(e => e.CountryId);
        }
    }
}
