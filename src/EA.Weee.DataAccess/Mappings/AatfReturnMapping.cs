namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class AatfReturnMapping : EntityTypeConfiguration<AatfReturn>
    {
        public AatfReturnMapping()
        {
            ToTable("AatfReturn", "AATF");
        }
    }
}
