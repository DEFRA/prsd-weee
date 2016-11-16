namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

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

            //HasRequired(ps => ps.MemberUpload)
            //    .WithMany(m => m.ProducerSubmissions)
            //    .Map(mc =>
            //    {
            //        mc.MapKey("MemberUploadId");
            //    });

            Ignore(ps => ps.ObligationType);
            Property(ps => ps.DatabaseObligationType).HasColumnName("ObligationType");
        }
    }
}