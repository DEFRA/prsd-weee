namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnsUploadRawDataMapping : EntityTypeConfiguration<DataReturnUploadRawData>
    {
        public DataReturnsUploadRawDataMapping()
        {
            ToTable("DataReturnUpload", "PCS");
        }
    }
}
