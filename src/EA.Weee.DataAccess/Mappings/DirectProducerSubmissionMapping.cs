namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    public class DirectProducerSubmissionConfiguration : EntityTypeConfiguration<DirectProducerSubmission>
    {
        public DirectProducerSubmissionConfiguration()
        {
            ToTable("DirectProducerSubmission", "Producer");

            Property(e => e.PaymentStatus).IsOptional();
            Property(e => e.PaymentReference).HasMaxLength(20).IsOptional();
            Property(e => e.PaymentId).HasMaxLength(35).IsOptional();
            Property(e => e.PaymentFinished).IsOptional();
            Property(e => e.PaymentReturnToken).HasMaxLength(35).IsOptional();
            Property(e => e.ComplianceYear).IsRequired();

            HasRequired(e => e.DirectRegistrant)
                .WithMany(er => er.DirectProducerSubmissions)
                .HasForeignKey(e => e.DirectRegistrantId);

            HasMany(e => e.SubmissionHistory)
                .WithRequired()
                .HasForeignKey(e => e.DirectProducerSubmissionId);
        }
    }
}
