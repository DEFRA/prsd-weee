﻿namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    public class DirectProducerSubmissionConfiguration : EntityTypeConfiguration<DirectProducerSubmission>
    {
        public DirectProducerSubmissionConfiguration()
        {
            ToTable("DirectProducerSubmission", "Producer");

            Property(e => e.PaymentFinished).IsOptional();
            
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
