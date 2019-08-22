namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class NonObligatedWeeeMapping : EntityTypeConfiguration<NonObligatedWeee>
    {
        public NonObligatedWeeeMapping()
        {
            ToTable("NonObligatedWeee", "AATF");

            Property(p => p.Tonnage).HasPrecision(28, 3);
        }
    }
}
