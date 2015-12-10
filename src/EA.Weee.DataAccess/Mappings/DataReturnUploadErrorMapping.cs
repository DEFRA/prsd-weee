namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.DataReturns;

    internal class DataReturnsUploadErrorrMapping : EntityTypeConfiguration<DataReturnUploadError>
    {
        public DataReturnsUploadErrorrMapping()
        {
            ToTable("DataReturnUploadError", "PCS");

            HasRequired<DataReturnUpload>(d => d.DataReturnUpload)
                .WithMany(d => d.Errors)
                .HasForeignKey(d => d.DataReturnUploadId);
        }
    }
}
