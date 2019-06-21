namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.AatfReturn;

    internal class AatfReturnReportOnMapping : EntityTypeConfiguration<ReturnReportOn>
    {
        public AatfReturnReportOnMapping()
        {
            ToTable("ReturnReportOn", "AATF");
        }
    }
}
