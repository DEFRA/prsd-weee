namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnUploadMapping : EntityTypeConfiguration<DataReturnUpload>
    {
        public DataReturnUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("DataReturnUpload", "PCS");
        }
    }
}
