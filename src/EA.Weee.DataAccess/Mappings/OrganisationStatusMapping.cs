namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class OrganisationStatusMapping : ComplexTypeConfiguration<OrganisationStatus>
    {
        public OrganisationStatusMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("OrganisationStatus");
        }
    }
}
