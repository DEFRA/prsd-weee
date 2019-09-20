namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnVersionMapping : EntityTypeConfiguration<DataReturnVersion>
    {
        public DataReturnVersionMapping()
        {
            ToTable("DataReturnVersion", "PCS");

            Ignore(dr => dr.IsSubmitted);
        }
    }
}
