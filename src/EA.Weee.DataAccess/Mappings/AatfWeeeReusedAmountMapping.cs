﻿namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeReusedAmountMapping : EntityTypeConfiguration<WeeeReusedAmount>
    {
        public AatfWeeeReusedAmountMapping()
        {
            ToTable("WeeeReusedAmount", "AATF");

            Property(x => x.CategoryId).HasColumnName("CategoryId").IsRequired();
            Property(x => x.HouseholdTonnage).HasColumnName("HouseholdTonnage").HasPrecision(28, 3);
            Property(x => x.NonHouseholdTonnage).HasColumnName("NonHouseholdTonnage").HasPrecision(28, 3);
        }
    }
}
