namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Producer;
    using System.Data.Entity.ModelConfiguration;

    public class DirectRegistrantConfiguration : EntityTypeConfiguration<DirectRegistrant>
    {
        public DirectRegistrantConfiguration()
        {
            ToTable("DirectRegistrant", "Producer");

            HasKey(e => e.Id);

            Property(e => e.RowVersion).IsRowVersion();

            HasRequired(e => e.Organisation).WithMany().HasForeignKey(e => e.OrganisationId);
            HasOptional(e => e.Contact).WithMany().HasForeignKey(e => e.ContactId);
            HasOptional(e => e.Address).WithMany().HasForeignKey(e => e.AddressId);
            HasOptional(e => e.SICCode).WithMany().HasForeignKey(e => e.SICCodeId);
            HasOptional(e => e.BrandName).WithMany().HasForeignKey(e => e.BrandNameId);
            HasOptional(e => e.AuthorisedRepresentative).WithMany().HasForeignKey(e => e.AuthorisedRepresentativeId);
        }
    }
}
