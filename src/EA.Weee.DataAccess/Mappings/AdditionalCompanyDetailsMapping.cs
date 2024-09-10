namespace EA.Weee.DataAccess.Mappings
{
    using EA.Weee.Domain.Organisation;
    using System.Data.Entity.ModelConfiguration;

    public class AdditionalCompanyDetailsConfiguration : EntityTypeConfiguration<AdditionalCompanyDetails>
    {
        public AdditionalCompanyDetailsConfiguration()
        {
            ToTable("AdditionalCompanyDetails", "Organisation");

            HasKey(e => e.Id);

            Property(e => e.FirstName).HasMaxLength(35).IsRequired();
            Property(e => e.LastName).HasMaxLength(35).IsRequired();
            Property(e => e.RowVersion).IsRowVersion();

            //HasRequired(e => e.DirectRegistrant).WithMany().HasForeignKey(e => e.DirectRegistrantId);
        }
    }
}
