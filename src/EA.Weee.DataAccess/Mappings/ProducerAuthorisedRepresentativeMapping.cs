namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    internal class ProducerAuthorisedRepresentativeMapping : EntityTypeConfiguration<AuthorisedRepresentative>
    {
        public ProducerAuthorisedRepresentativeMapping()
        {
            ToTable("AuthorisedRepresentative", "Producer");
        }
    }
}
