namespace EA.Weee.DataAccess.Mappings
{
    using Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    internal class OrganisationAdditionalDetailsTypeMapping : ComplexTypeConfiguration<OrganisationAdditionalDetailsType>
    {
        public OrganisationAdditionalDetailsTypeMapping()
        {
            Ignore(x => x.DisplayName);
            Property(x => x.Value).HasColumnName("Type");
        }
    }
}
