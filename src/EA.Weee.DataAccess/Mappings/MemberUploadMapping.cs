namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class MemberUploadMapping : EntityTypeConfiguration<MemberUpload>
    {
        public MemberUploadMapping()
        {
            ToTable("MemberUpload", "PCS");
        }
    }
}
