namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

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
