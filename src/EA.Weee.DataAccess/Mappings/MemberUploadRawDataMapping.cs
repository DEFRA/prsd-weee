namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class MemberUploadRawDataMapping : EntityTypeConfiguration<MemberUploadRawData>
    {
        public MemberUploadRawDataMapping()
        {
            ToTable("MemberUpload", "PCS");
        }
    }
}
