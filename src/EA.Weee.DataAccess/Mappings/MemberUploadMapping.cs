namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Scheme;
    using System.Data.Entity.ModelConfiguration;

    internal class MemberUploadMapping : EntityTypeConfiguration<MemberUpload>
    {
        public MemberUploadMapping()
        {
            HasRequired(e => e.RawData).WithRequiredPrincipal();

            ToTable("MemberUpload", "PCS");
        }
    }
}
