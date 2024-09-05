namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class DirectProducerSubmissionStatusMapping : ComplexTypeConfiguration<DirectProducerSubmissionStatus>
    {
        public DirectProducerSubmissionStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Status");
        }
    }
}
