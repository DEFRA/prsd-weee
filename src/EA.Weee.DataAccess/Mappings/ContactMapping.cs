namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain.Organisation;

    internal class ContactMapping : EntityTypeConfiguration<Contact>
    {
        public ContactMapping()
        {
            ToTable("Contact", "Organisation");

            Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(35);
            Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(35);
            Property(x => x.Position).HasColumnName("Position").IsRequired().HasMaxLength(35);
        }
    }
}
