namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationTransactionMapping : EntityTypeConfiguration<OrganisationTransaction>
    {
        public OrganisationTransactionMapping()
        {
            ToTable("OrganisationTransaction", "Organisation");
            Property(o => o.OrganisationJson).IsRequired();
            Property(o => o.UserId).IsRequired();
            Property(o => o.CreatedDateTime).IsRequired();
            Property(o => o.CompletedDateTime).IsOptional();
        }
    }
}
