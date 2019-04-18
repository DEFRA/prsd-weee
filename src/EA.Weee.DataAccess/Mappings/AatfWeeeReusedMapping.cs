namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.AatfReturn;

    internal class AatfWeeeReusedMapping : EntityTypeConfiguration<WeeeReused>
    {
        public AatfWeeeReusedMapping()
        {
            ToTable("WeeeReused", "AATF");
        }
    }
}
