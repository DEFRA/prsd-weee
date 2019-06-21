namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    public class AatfWeeeSentOnAmountMapping : EntityTypeConfiguration<WeeeSentOnAmount>
    {
        public AatfWeeeSentOnAmountMapping()
        {
            ToTable("WeeeSentOnAmount", "AATF");

            Property(x => x.CategoryId).HasColumnName("CategoryId").IsRequired();
            Property(x => x.HouseholdTonnage).HasColumnName("HouseholdTonnage").HasPrecision(28, 3);
            Property(x => x.NonHouseholdTonnage).HasColumnName("NonHouseholdTonnage").HasPrecision(28, 3);
        }
    }
}
