namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using Domain;

    internal class ContactMapping : ComplexTypeConfiguration<Contact>
    {
        public ContactMapping()
        {
            Property(x => x.FirstName).HasColumnName("FirstName");
            Property(x => x.LastName).HasColumnName("LastName");
            Property(x => x.Telephone).HasColumnName("Telephone");
            Property(x => x.Fax).HasColumnName("Fax");
            Property(x => x.Email).HasColumnName("Email");
        }
    }
}
