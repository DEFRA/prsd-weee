namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnUploadRawDataMapping : EntityTypeConfiguration<DataReturnUploadRawData>
    {
        public DataReturnUploadRawDataMapping()
        {
            ToTable("DataReturnUpload", "PCS");
        }
    }
}
