namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnVersionMapping : EntityTypeConfiguration<DataReturnVersion>
    {
        public DataReturnVersionMapping()
        {
            ToTable("DataReturnVersion", "PCS");

            HasRequired(drv => drv.DataReturn)
                .WithOptional(drv => drv.CurrentDataReturnVersion)
                .Map(mc =>
                {
                    mc.MapKey("DataReturnId");
                });
            Ignore(dr => dr.IsSubmitted);
        }
    }
}
