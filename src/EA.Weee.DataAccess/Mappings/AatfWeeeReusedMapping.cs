namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeReusedMapping : EntityTypeConfiguration<WeeeReused>
    {
        public AatfWeeeReusedMapping()
        {
            ToTable("WeeeReused", "AATF");
        }
    }
}
