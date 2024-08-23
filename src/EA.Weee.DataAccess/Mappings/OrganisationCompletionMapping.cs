namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationCompletionMapping : ComplexTypeConfiguration<CompletionStatus>
    {
        public OrganisationCompletionMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("CompletionStatus");
        }
    }
}
