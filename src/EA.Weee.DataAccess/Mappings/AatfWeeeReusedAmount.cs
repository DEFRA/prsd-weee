namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeReusedAmount : EntityTypeConfiguration<WeeeReusedAmount>
    {
        public AatfWeeeReusedAmount()
        {
            ToTable("WeeeReusedAmount", "AATF");

            Property(x => x.CategoryId).HasColumnName("CategoryId").IsRequired();
            Property(x => x.HouseholdTonnage).HasColumnName("HouseholdTonnage").HasPrecision(28, 3);
            Property(x => x.NonHouseholdTonnage).HasColumnName("NonHouseholdTonnage").HasPrecision(28, 3);
        }
    }
}
