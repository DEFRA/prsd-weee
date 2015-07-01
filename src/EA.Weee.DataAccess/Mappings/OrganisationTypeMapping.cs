namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class OrganisationTypeMapping : ComplexTypeConfiguration<OrganisationType>
    {
        public OrganisationTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationType");
        }
    }
}
