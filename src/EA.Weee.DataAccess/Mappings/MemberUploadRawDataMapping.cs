namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class MemberUploadRawDataMapping : EntityTypeConfiguration<MemberUploadRawData>
    {
        public MemberUploadRawDataMapping()
        {
            ToTable("MemberUpload", "PCS");
        }
    }
}
