namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnUploadMapping : EntityTypeConfiguration<DataReturnUpload>
    {
        public DataReturnUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("DataReturnUpload", "PCS");
        }
    }
}
