namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class MemberUploadErrorMapping : EntityTypeConfiguration<MemberUploadError>
    {
        public MemberUploadErrorMapping()
        {
            ToTable("MemberUploadError", "PCS");

            HasRequired<MemberUpload>(m => m.MemberUpload)
                .WithMany(m => m.Errors)
                .HasForeignKey(m => m.MemberUploadId);
        }
    }
}
