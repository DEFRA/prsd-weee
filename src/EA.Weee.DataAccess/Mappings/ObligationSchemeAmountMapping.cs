﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Obligation;
    using System.Data.Entity.ModelConfiguration;

    internal class ObligationSchemeAmountMapping : EntityTypeConfiguration<ObligationSchemeAmount>
    {
        public ObligationSchemeAmountMapping()
        {
            ToTable("ObligationSchemeAmount", "PCS");
            HasKey(o => o.Id);
            Property(n => n.CategoryId).IsRequired().HasColumnName("CategoryId");
            Property(n => n.Obligation).IsOptional().HasPrecision(28, 3).HasColumnName("Obligation");

            HasRequired(o => o.ObligationScheme).WithMany(o => o.ObligationSchemeAmounts).HasForeignKey(o => o.ObligationSchemeId);
        }
    }
}
