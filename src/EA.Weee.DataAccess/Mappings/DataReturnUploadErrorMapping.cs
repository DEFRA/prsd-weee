namespace EA.Weee.DataAccess.Mappings
{
    using Domain.DataReturns;
    using System.Data.Entity.ModelConfiguration;

    internal class DataReturnUploadErrorMapping : EntityTypeConfiguration<DataReturnUploadError>
    {
        public DataReturnUploadErrorMapping()
        {
            ToTable("DataReturnUploadError", "PCS");

            HasRequired<DataReturnUpload>(d => d.DataReturnUpload)
                .WithMany(d => d.Errors)
                .HasForeignKey(d => d.DataReturnUploadId);
        }
    }
}
