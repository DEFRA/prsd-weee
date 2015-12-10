namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnMapping : EntityTypeConfiguration<DataReturn>
    {
        public DataReturnMapping()
        {
            ToTable("DataReturn", "PCS");
        }
    }
}
