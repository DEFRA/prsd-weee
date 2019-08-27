namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class AatfReturnReportOnMapping : EntityTypeConfiguration<ReturnReportOn>
    {
        public AatfReturnReportOnMapping()
        {
            ToTable("ReturnReportOn", "AATF");
        }
    }
}
