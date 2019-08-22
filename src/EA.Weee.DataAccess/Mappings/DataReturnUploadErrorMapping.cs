namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

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
