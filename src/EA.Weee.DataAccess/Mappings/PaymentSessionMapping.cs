namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class PaymentSessionConfiguration : EntityTypeConfiguration<PaymentSession>
    {
        public PaymentSessionConfiguration()
        {
            ToTable("PaymentSession", "Producer");

            Property(e => e.UserId).IsRequired().HasMaxLength(128);
            Property(e => e.PaymentId).IsRequired().HasMaxLength(35);
            Property(e => e.PaymentReference).IsRequired().HasMaxLength(20);
            Property(e => e.PaymentReturnToken).IsRequired().HasMaxLength(150);
            Property(e => e.Amount).HasPrecision(18, 2);
            Property(e => e.UpdatedById).IsOptional();
            Property(e => e.RowVersion).IsRowVersion();

            HasRequired(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            HasRequired(e => e.DirectRegistrant)
                .WithMany()
                .HasForeignKey(e => e.DirectRegistrantId);

            HasRequired(e => e.DirectProducerSubmission)
                .WithMany()
                .HasForeignKey(e => e.DirectProducerSubmissionId);

            HasOptional(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById);
        }
    }
}
