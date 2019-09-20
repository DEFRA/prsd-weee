namespace EA.Weee.DataAccess.Mappings
{
    using Domain.AatfReturn;
    using System.Data.Entity.ModelConfiguration;

    internal class ReturnMapping : EntityTypeConfiguration<Return>
    {
        public ReturnMapping()
        {
            ToTable("Return", "AATF");

            Property(dr => dr.Quarter.Year).HasColumnName("ComplianceYear");
            Property(dr => dr.Quarter.Q).HasColumnName("Quarter");
        }
    }
}
