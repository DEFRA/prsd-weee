namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class OrganisationMapping : EntityTypeConfiguration<Organisation>
    {
        public OrganisationMapping()
        {
            ToTable("Organisation", "Organisation");

            HasOptional(t => t.Contact).WithOptionalDependent();
            HasOptional(t => t.OrganisationAddress).WithOptionalDependent();
            HasOptional(t => t.BusinessAddress).WithOptionalDependent();
            HasOptional(t => t.NotificationAddress).WithOptionalDependent();
        }
    }
}