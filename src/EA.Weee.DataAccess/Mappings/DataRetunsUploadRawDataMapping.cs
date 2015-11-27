namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnsUploadRawDataMapping : EntityTypeConfiguration<DataReturnsUploadRawData>
    {
        public DataReturnsUploadRawDataMapping()
        {
            ToTable("DataReturnsUpload", "PCS");
        }
    }
}
