namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    public class DirectProducerSubmissionConfiguration : EntityTypeConfiguration<DirectProducerSubmission>
    {
        public DirectProducerSubmissionConfiguration()
        {
            ToTable("DirectProducerSubmission", "Producer");

            HasKey(e => e.Id);

            Property(e => e.CreatedDate).IsRequired();
            Property(e => e.CreatedById).HasMaxLength(128).IsRequired();
            Property(e => e.UpdatedById).HasMaxLength(128);
            Property(e => e.PaymentReference).HasMaxLength(20);
            Property(e => e.PaymentId).HasMaxLength(20);

            HasRequired(e => e.DirectRegistrant).WithMany().HasForeignKey(e => e.DirectRegistrantId);
            HasOptional(e => e.ServiceOfNoticeAddress).WithMany().HasForeignKey(e => e.ServiceOfNoticeAddressId);
            HasOptional(e => e.AppropriateSignatory).WithMany().HasForeignKey(e => e.AppropriateSignatoryId);
            HasRequired(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
            HasOptional(e => e.UpdatedBy).WithMany().HasForeignKey(e => e.UpdatedById);
        }
    }
}
