namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfWeeeReusedSiteMapping : EntityTypeConfiguration<WeeeReusedSite>
    {
        public AatfWeeeReusedSiteMapping()
        {
            ToTable("WeeeReusedSite", "AATF");
        }
    }
}
