﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfContactMapping : EntityTypeConfiguration<AatfContact>
    {
        public AatfContactMapping()
        {
            ToTable("Contact", "AATF");

            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(35);
            Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(35);
            Property(x => x.Position).HasColumnName("Position").HasMaxLength(35);
            Property(x => x.Address1).HasColumnName("Address1").IsRequired().HasMaxLength(60);
            Property(x => x.Address2).HasColumnName("Address2").HasMaxLength(60);
            Property(x => x.TownOrCity).HasColumnName("TownOrCity").IsRequired().HasMaxLength(35);
            Property(x => x.CountyOrRegion).HasColumnName("CountyOrRegion").HasMaxLength(35);
            Property(x => x.Postcode).HasColumnName("Postcode").HasMaxLength(10);
            Property(x => x.CountryId).HasColumnName("CountryId").IsRequired();
            Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(256);
            Property(x => x.Telephone).HasColumnName("Telephone").HasMaxLength(20);
        }
    }
}
