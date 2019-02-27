namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.AatfReturn;

    internal class AatfWeeeReusedSiteMapping : EntityTypeConfiguration<WeeeReusedSite>
    {
        public AatfWeeeReusedSiteMapping()
        {
            ToTable("WeeeReusedSite", "AATF");
        }
    }
}
