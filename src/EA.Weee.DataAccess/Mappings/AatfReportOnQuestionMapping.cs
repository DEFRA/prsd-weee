namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.AatfReturn;

    internal class AatfReportOnQuestionMapping : EntityTypeConfiguration<ReportOnQuestion>
    {
        public AatfReportOnQuestionMapping()
        {
            ToTable("ReportOnQuestion", "AATF");
        }
    }
}
