namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class DataReturnsUploadErrorrMapping : EntityTypeConfiguration<DataReturnsUploadError>
    {
        public DataReturnsUploadErrorrMapping()
        {
            ToTable("DataReturnsUploadError", "PCS");

            HasRequired<DataReturnsUpload>(d => d.DataReturnsUpload)
                .WithMany(d => d.Errors)
                .HasForeignKey(d => d.DataReturnsUploadId);
        }
    }
}
