namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain.AatfReturn;

    internal class AatfReturnReportOn : EntityTypeConfiguration<ReturnReportOn>
    {
        public AatfReturnReportOn()
        {
            ToTable("ReturnReportOn", "AATF");
        }
    }
}
