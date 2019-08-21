namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnMapping : EntityTypeConfiguration<DataReturn>
    {
        public DataReturnMapping()
        {
            ToTable("DataReturn", "PCS");

            Property(dr => dr.Quarter.Year).HasColumnName("ComplianceYear");
            Property(dr => dr.Quarter.Q).HasColumnName("Quarter");

            HasOptional(dr => dr.CurrentVersion)
                .WithMany()
                .Map(mc =>
                {
                    mc.MapKey("CurrentDataReturnVersionId");
                });
        }
    }
}
