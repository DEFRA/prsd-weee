﻿namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    public class DirectProducerSubmissionHistoryMapping : EntityTypeConfiguration<DirectProducerSubmissionHistory>
    {
        public DirectProducerSubmissionHistoryMapping()
        {
            ToTable("DirectProducerSubmissionHistory", "Producer");

            Property(e => e.DirectProducerSubmissionId).IsRequired();
            Property(e => e.ServiceOfNoticeAddressId).IsOptional();
            Property(e => e.AppropriateSignatoryId).IsOptional();
            Property(e => e.SubmittedDate).IsOptional();
            Property(e => e.CompanyName).HasMaxLength(256).IsOptional();
            Property(e => e.TradingName).HasMaxLength(256).IsOptional();

            HasRequired(e => e.DirectProducerSubmission)
                .WithMany(ed => ed.SubmissionHistory)
                .HasForeignKey(e => e.DirectProducerSubmissionId);

            HasOptional(e => e.ServiceOfNoticeAddress)
                .WithMany()
                .HasForeignKey(e => e.ServiceOfNoticeAddressId);

            HasOptional(e => e.AppropriateSignatory)
                .WithMany()
                .HasForeignKey(e => e.AppropriateSignatoryId);

            HasOptional(e => e.Contact).WithMany().HasForeignKey(e => e.ContactId);
            HasOptional(e => e.BusinessAddress).WithMany().HasForeignKey(e => e.BusinessAddressId);
            HasOptional(e => e.ContactAddress).WithMany().HasForeignKey(e => e.ContactAddressId);
            HasOptional(e => e.BrandName).WithMany().HasForeignKey(e => e.BrandNameId);
            HasOptional(e => e.AuthorisedRepresentative).WithMany().HasForeignKey(e => e.AuthorisedRepresentativeId);
        }
    }
}
