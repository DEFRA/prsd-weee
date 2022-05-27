namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Obligation;

    internal class ObligationUploadErrorMapping : EntityTypeConfiguration<ObligationUploadError>
    {
        public ObligationUploadErrorMapping()
        {
            ToTable("ObligationUploadErrorMapping", "PCS");
            HasKey(o => o.Id);
            Property(o => o.ErrorType.Value).HasColumnName("ErrorType").IsRequired();
            Property(o => o.SchemeIdentifier).IsRequired().HasColumnName("SchemeIdentifier").IsUnicode();
            Property(o => o.SchemeName).IsRequired().HasColumnName("SchemeName").IsUnicode();
            Property(o => o.Description).IsRequired().HasColumnName("Description");
            Property(o => o.Category).IsRequired().HasColumnName("Category");

            HasRequired(o => o.ObligationUpload).WithMany(o => o.ObligationUploadErrors).HasForeignKey(o => o.ObligationUploadId);
        }
    }
}
