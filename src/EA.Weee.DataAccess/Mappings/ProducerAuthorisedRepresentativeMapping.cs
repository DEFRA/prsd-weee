namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Producer;

    internal class ProducerAuthorisedRepresentativeMapping : EntityTypeConfiguration<AuthorisedRepresentative>
    {
        public ProducerAuthorisedRepresentativeMapping()
        {
            ToTable("AuthorisedRepresentative", "Producer");
        }
    }
}
