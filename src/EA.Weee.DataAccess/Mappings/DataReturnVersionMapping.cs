namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnVersionMapping : EntityTypeConfiguration<DataReturnVersion>
    {
        public DataReturnVersionMapping()
        {
            ToTable("DataReturnVersion", "PCS");
            HasMany(d => d.WeeeCollectedReturnVersions);

            Ignore(dr => dr.IsSubmitted);
        }
    }
}
