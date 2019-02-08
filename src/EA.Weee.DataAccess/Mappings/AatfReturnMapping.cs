namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfReturnMapping : EntityTypeConfiguration<AatfReturn>
    {
        public AatfReturnMapping()
        {
            ToTable("AatfReturn", "AATF");
        }
    }
}
