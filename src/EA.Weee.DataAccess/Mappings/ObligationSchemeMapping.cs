namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Obligation;

    internal class ObligationSchemeMapping : EntityTypeConfiguration<ObligationScheme>
    {
        public ObligationSchemeMapping()
        {
            ToTable("ObligationScheme", "PCS");
            HasKey(o => o.Id);
            Property(o => o.ObligationUploadId).IsRequired().HasColumnName("ObligationUploadId");
            Property(o => o.ComplianceYear).IsRequired().HasColumnName("ComplianceYear");
            Property(o => o.UpdatedDate).IsRequired().HasColumnName("UpdatedDate");
            Property(o => o.SchemeId).IsRequired().HasColumnName("SchemeId");
            Property(n => n.CategoryId).IsRequired().HasColumnName("CategoryId");
            Property(n => n.Obligation).IsOptional().HasPrecision(28, 3).HasColumnName("Obligation");
            
            HasRequired(o => o.Scheme);
            HasRequired(o => o.ObligationUpload).WithMany(o => o.ObligationSchemes).HasForeignKey(o => o.ObligationUploadId);
        }
    }
}
