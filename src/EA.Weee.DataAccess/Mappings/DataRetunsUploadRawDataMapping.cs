namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class DataReturnsUploadRawDataMapping : EntityTypeConfiguration<DataReturnsUploadRawData>
    {
        public DataReturnsUploadRawDataMapping()
        {
            ToTable("DataReturnsUpload", "PCS");
        }
    }
}
