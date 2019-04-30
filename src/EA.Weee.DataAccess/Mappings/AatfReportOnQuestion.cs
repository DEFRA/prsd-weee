namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.AatfReturn;

    internal class AatfReportOnQuestion : EntityTypeConfiguration<ReportOnQuestion>
    {
        public AatfReportOnQuestion()
        {
            ToTable("ReportOnQuestion", "AATF");
        }
    }
}
