namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerSubmissionMapping : EntityTypeConfiguration<ProducerSubmission>
    {
        public ProducerSubmissionMapping()
        {
            ToTable("ProducerSubmission", "Producer");
            Ignore(p => p.OrganisationName);
            Property(p => p.AnnualTurnover).HasPrecision(28, 12);

            HasRequired(ps => ps.RegisteredProducer)
                .WithMany(rp => rp.ProducerSubmissions)
                .Map(mc =>
                {
                    mc.MapKey("RegisteredProducerId");
                });

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");
        }
    }
}