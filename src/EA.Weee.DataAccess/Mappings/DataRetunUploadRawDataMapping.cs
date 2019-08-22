namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnsUploadRawDataMapping : EntityTypeConfiguration<DataReturnUploadRawData>
    {
        public DataReturnsUploadRawDataMapping()
        {
            ToTable("DataReturnUpload", "PCS");
        }
    }
}
