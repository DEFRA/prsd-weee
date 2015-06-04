namespace EA.Weee.DataAccess.Mappings
{
    using System.Data.Entity.ModelConfiguration;
    using EA.Weee.Domain;

    internal class ContactMapping : EntityTypeConfiguration<Contact>
    {
        public ContactMapping()
        {
            ToTable("Contact", "Organisation");
        }
    }
}
