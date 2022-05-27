namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Obligation;

    internal class ObligationUploadMapping : EntityTypeConfiguration<ObligationUpload>
    {
        public ObligationUploadMapping()
        {
            ToTable("ObligationUpload", "PCS");
            HasKey(o => o.Id);
            Property(o => o.Data).IsRequired().HasColumnName("Data").IsUnicode();
            Property(o => o.FileName).IsRequired().HasColumnName("FileName").IsUnicode();
            Property(o => o.UploadedDate).IsRequired().HasColumnName("UploadedDate");
            Property(o => o.UploadedById).IsRequired().HasColumnName("UploadedById");
        }
    }
}
