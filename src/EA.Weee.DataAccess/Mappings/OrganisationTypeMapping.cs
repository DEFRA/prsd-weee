namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class OrganisationTypeMapping : ComplexTypeConfiguration<OrganisationType>
    {
        public OrganisationTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationType");
        }
    }
}
