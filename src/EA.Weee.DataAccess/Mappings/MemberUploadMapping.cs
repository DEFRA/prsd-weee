namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Scheme;

    internal class MemberUploadMapping : EntityTypeConfiguration<MemberUpload>
    {
        public MemberUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("MemberUpload", "PCS");
        }
    }
}
